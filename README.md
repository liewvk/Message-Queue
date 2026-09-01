# Message Queue - Visual Basic 2026 Programming

A comprehensive demonstration of asynchronous messaging patterns in Visual Basic .NET, showcasing both **Point-to-Point Queue** and **Publish/Subscribe Event Bus** architectures.

## 📋 Overview

This project illustrates two fundamental messaging patterns used in modern distributed systems:

1. **Point-to-Point Queue (Channel-based)**: Direct, one-to-one message delivery where each message is processed exactly once.
2. **Publish/Subscribe Event Bus**: One-to-many message delivery where a single event reaches multiple independent subscribers.

## 🎯 Key Concepts

### Point-to-Point Queue
- **One Producer, One Consumer**: Messages flow in a queue from a single sender to a single receiver.
- **Exactly-Once Delivery**: Each message is guaranteed to be processed exactly once.
- **Implementation**: Uses `System.Threading.Channels.Channel(Of T)` for in-memory, thread-safe message passing.
- **Use Case**: Order processing, task queuing, job scheduling.

### Publish/Subscribe Event Bus
- **One Publisher, Many Subscribers**: A single event reaches all registered subscribers independently.
- **Loose Coupling**: Publishers and subscribers don't need to know about each other.
- **Concurrent Execution**: All subscribers are notified and executed simultaneously.
- **Use Case**: Email notifications, inventory updates, audit logging, analytics.

## 🏗️ Project Structure

```
Message-Queue/
├── Program.vb              # Main demonstration and implementations
├── Message Queue.vbproj    # Visual Basic project file
├── Message Queue.slnx      # Solution file
├── .gitignore             # Git ignore rules
├── .gitattributes         # Git attributes
└── README.md              # This file
```

## 📚 Core Classes

### `OrderPlacedEvent`
Represents a business event in the system.
```vb
Public Class OrderPlacedEvent
    Public Property OrderId As Integer
    Public Property CustomerName As String
End Class
```

### `OrderQueue`
Implements a point-to-point queue using `Channel(Of OrderPlacedEvent)`.
- **Bounded Capacity**: Limited to 1,000 messages to prevent unbounded memory growth.
- **Thread-Safe**: Built-in synchronization for producer-consumer scenarios.

### `EventBus`
Implements a publish/subscribe event bus pattern.
- **Type-Based Routing**: Events are routed to subscribers based on their type.
- **Generic Subscribers**: Supports any event type via generics.
- **Async Handlers**: All subscriber handlers are asynchronous for non-blocking execution.

## 🚀 How It Works

### Demo 1: Point-to-Point Queue

```
Producer (Writes 5 Orders)
         ↓
    [Queue Channel]
         ↓
Consumer (Processes Each Once)
```

1. Producer writes 5 orders to the queue with 50ms delays between writes.
2. Consumer reads from the queue and processes each order with 10ms processing time.
3. Channel signals completion after all orders are written.
4. Consumer drains the queue and processes all messages.

**Output Example:**
```
--- Point-to-Point Queue (Channel) ---
  [Producer] Wrote OrderId=1 (Customer 1)
  [Producer] Wrote OrderId=2 (Customer 2)
  [Consumer] Processed OrderId=1 (Customer 1)
  [Consumer] Processed OrderId=2 (Customer 2)
  ...
  Queue drained — all orders processed exactly once.
```

### Demo 2: Publish/Subscribe Event Bus

```
Publisher (Publishes 1 OrderPlacedEvent)
              ↓
        [EventBus]
         ↙   ↓   ↘
   Email  Inventory  Audit
   Service  Service   Log
```

1. Publisher publishes a single `OrderPlacedEvent`.
2. Three independent subscribers react simultaneously:
   - **EmailService**: Sends confirmation email (20ms delay)
   - **InventoryService**: Reserves stock (15ms delay)
   - **AuditLog**: Logs the event (5ms delay)
3. All subscribers complete their work concurrently.

**Output Example:**
```
--- Publish/Subscribe (EventBus) ---
  [Publisher] Publishing OrderPlacedEvent for Order #101
  [EmailService] Sending confirmation to Aina Zulkifli for Order #101
  [InventoryService] Reserving stock for Order #101
  [AuditLog] Logged OrderPlacedEvent for Order #101 at 2026-09-01T12:34:56.7890000Z
  All subscribers notified.
```

## 🔧 Technical Details

### Async/Await Pattern
- All operations use `Async`/`Await` for non-blocking I/O.
- Demonstrates proper task orchestration with `Task.WhenAll()`.

### Channel API (`System.Threading.Channels`)
- Modern .NET API for producer-consumer scenarios.
- Bounded channels prevent memory exhaustion.
- Non-blocking write/read operations.

### Generic Programming
- `EventBus` uses generics (`Of T`) for type-safe event handling.
- Supports any event type without code changes.

## 💻 Running the Project

### Prerequisites
- Visual Studio 2026 or later
- .NET runtime (compatible with Visual Basic 2026)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/liewvk/Message-Queue.git
   cd Message-Queue
   ```

2. Open the solution:
   ```bash
   start "Message Queue.slnx"
   ```

3. Build and Run:
   - Press `F5` to build and run
   - Or use the Command Palette: `dotnet run`

4. Expected Output:
   - Both demos will execute sequentially
   - Press any key to exit after completion

## 🎓 Learning Outcomes

By studying this project, you will understand:

- ✅ Async/await patterns in Visual Basic .NET
- ✅ Task parallelism and `Task.WhenAll()` orchestration
- ✅ Point-to-point messaging with `Channel(Of T)`
- ✅ Publish/subscribe event bus architecture
- ✅ Loose coupling between components
- ✅ Thread-safe, in-memory messaging
- ✅ Generic programming for flexible event handling

## 🔗 Related Patterns

- **Producer-Consumer Pattern**: OrderQueue implements this.
- **Observer Pattern**: EventBus implements this.
- **Async Event Processing**: Non-blocking, concurrent event handling.
- **Pub/Sub Messaging**: Foundation for modern microservices.

## 📝 License

This project is part of Visual Basic 2026 Programming coursework.

## 👨‍💻 Author

**liewvk** - Visual Basic 2026 Programming Student

## 🤝 Contributing

For educational improvements or questions:
- Open an Issue
- Submit a Pull Request
- Check the Discussions tab

## 📞 Support

For issues or questions:
1. Check existing issues on GitHub
2. Review the code comments and documentation
3. Open a new issue with detailed description

---

**Version**: 22.1  
**Last Updated**: 2026-09-01  
**Language**: Visual Basic .NET
