using System.Text;
using RabbitMQ.Client;

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}

Console.WriteLine("[START]");

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// Durable property makes the queue persist even if RabbitMQ node restarts
channel.QueueDeclare(queue: "test-queue2",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var message = GetMessage(args);

var body = Encoding.UTF8.GetBytes(message);

// This will mark messages as persistent in the queue, which means
// that if it wasn't consumed and you restart the RabbitMQ node, it will
// remain on the queue (the queue must be durable).
// If using RabbitMQ within a container, ensure to configure the volume
// so it can persist data in the hard disk.
var properties = channel.CreateBasicProperties();
properties.Persistent = true;

// Routing key must be the same name as queue
channel.BasicPublish(exchange: String.Empty,
                routingKey: "test-queue2",
                basicProperties: null,
                body: body);

Console.WriteLine($"Sent message: {message}");

Console.WriteLine("[END] Press any key to close...");
Console.ReadKey();