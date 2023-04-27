using System.Text;
using System.Text.Json;
using Consumer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("[START]");

var factory = new ConnectionFactory
{
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "serialization-queue",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();

    var message = Encoding.UTF8.GetString(body);

    var myObject = JsonSerializer.Deserialize<chuchu>(message);
    Console.WriteLine($" [x] My object name {myObject.Name}");
    Console.WriteLine($" [x] My object author {myObject.Author}");
    Console.WriteLine($" [x] My object id {myObject.Id}");
    Console.WriteLine($" [x] Received {message}");
};

channel.BasicConsume(queue: "serialization-queue",
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();