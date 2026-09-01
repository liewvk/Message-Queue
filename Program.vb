Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Channels
Imports System.Threading.Tasks

Module MessagingDemo

    Sub Main()
        RunDemo().GetAwaiter().GetResult()
    End Sub

    Async Function RunDemo() As Task
        Console.WriteLine("=== Point-to-Point Queue vs Pub/Sub Event Bus Demo ===" & Environment.NewLine)

        Await RunQueueDemo()
        Console.WriteLine()
        Await RunEventBusDemo()

        Console.WriteLine(Environment.NewLine & "Press any key to exit...")
        Console.ReadKey()
    End Function

    ' ============================================================
    ' DEMO 1: Point-to-point queue (Channel(Of T))
    ' One producer, one consumer — each message is handled exactly once.
    ' ============================================================
    Async Function RunQueueDemo() As Task
        Console.WriteLine("--- Point-to-Point Queue (Channel) ---")

        Dim orderQueue As New OrderQueue()
        Await ConsumeAndProduce(orderQueue)
    End Function

    Async Function ConsumeAndProduce(orderQueue As OrderQueue) As Task
        ' Producer: writes 5 orders onto the queue.
        Dim producer As Task = Task.Run(
            Async Function() As Task
                For i As Integer = 1 To 5
                    Dim evt As New OrderPlacedEvent With {
                        .OrderId = i,
                        .CustomerName = $"Customer {i}"
                    }
                    Await orderQueue.Queue.Writer.WriteAsync(evt)
                    Console.WriteLine($"  [Producer] Wrote OrderId={evt.OrderId} ({evt.CustomerName})")
                    Await Task.Delay(50) ' simulate spacing between orders
                Next
                orderQueue.Queue.Writer.Complete() ' signals no more items
            End Function)

        ' Consumer: reads until the channel is completed and drained.
        Dim consumer As Task = Task.Run(
            Async Function() As Task
                Dim reader As ChannelReader(Of OrderPlacedEvent) = orderQueue.Queue.Reader
                While Await reader.WaitToReadAsync()
                    Dim evt As OrderPlacedEvent = Nothing
                    While reader.TryRead(evt)
                        Await Task.Delay(10) ' simulate processing time
                        Console.WriteLine($"  [Consumer] Processed OrderId={evt.OrderId} ({evt.CustomerName})")
                    End While
                End While
            End Function)

        Await Task.WhenAll(producer, consumer)
        Console.WriteLine("  Queue drained — all orders processed exactly once.")
    End Function

    ' ============================================================
    ' DEMO 2: Publish/subscribe event bus
    ' One publish reaches every subscriber independently.
    ' ============================================================
    Async Function RunEventBusDemo() As Task
        Console.WriteLine("--- Publish/Subscribe (EventBus) ---")

        Dim bus As New EventBus()

        ' Subscriber 1: simulate sending a confirmation email.
        bus.Subscribe(Of OrderPlacedEvent)(
            Async Function(evt) As Task
                Await Task.Delay(20)
                Console.WriteLine($"  [EmailService] Sending confirmation to {evt.CustomerName} for Order #{evt.OrderId}")
            End Function)

        ' Subscriber 2: simulate updating inventory.
        bus.Subscribe(Of OrderPlacedEvent)(
            Async Function(evt) As Task
                Await Task.Delay(15)
                Console.WriteLine($"  [InventoryService] Reserving stock for Order #{evt.OrderId}")
            End Function)

        ' Subscriber 3: simulate an analytics/audit log.
        bus.Subscribe(Of OrderPlacedEvent)(
            Async Function(evt) As Task
                Await Task.Delay(5)
                Console.WriteLine($"  [AuditLog] Logged OrderPlacedEvent for Order #{evt.OrderId} at {DateTime.UtcNow:O}")
            End Function)

        ' Publish a single event — all three subscribers react independently.
        Dim placedOrder As New OrderPlacedEvent With {
            .OrderId = 101,
            .CustomerName = "Aina Zulkifli"
        }

        Console.WriteLine($"  [Publisher] Publishing OrderPlacedEvent for Order #{placedOrder.OrderId}")
        Await bus.PublishAsync(placedOrder)
        Console.WriteLine("  All subscribers notified.")
    End Function

End Module

' ============================================================
' EVENT
' ============================================================

Public Class OrderPlacedEvent
    Public Property OrderId As Integer
    Public Property CustomerName As String
End Class

' ============================================================
' POINT-TO-POINT QUEUE
'
' Channel(Of T) acts as an in-memory queue.
' Writer = producer
' Reader = consumer
' ============================================================

Public Class OrderQueue

    Public ReadOnly Queue As Channel(Of OrderPlacedEvent)

    Public Sub New()
        Queue = Channel.CreateBounded(Of OrderPlacedEvent)(1000)
    End Sub

End Class

' ============================================================
' PUBLISH/SUBSCRIBE EVENT BUS
'
' One publisher can send an event to many subscribers.
' ============================================================

Public Class EventBus

    Private ReadOnly _handlers As New Dictionary(
        Of Type,
        List(Of Func(Of Object, Task)))()

    ' Register a subscriber.
    Public Sub Subscribe(Of T)(handler As Func(Of T, Task))

        Dim eventType As Type = GetType(T)

        If Not _handlers.ContainsKey(eventType) Then
            _handlers(eventType) = New List(Of Func(Of Object, Task))()
        End If

        _handlers(eventType).Add(
            Function(message As Object) handler(DirectCast(message, T)))

    End Sub

    ' Publish an event to every registered subscriber.
    Public Async Function PublishAsync(Of T)(message As T) As Task

        Dim handlers As List(Of Func(Of Object, Task)) = Nothing

        If _handlers.TryGetValue(GetType(T), handlers) Then
            Dim tasks = handlers.Select(Function(h) h(message))
            Await Task.WhenAll(tasks)
        End If

    End Function

End Class
