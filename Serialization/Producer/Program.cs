using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Producer;
using RabbitMQ.Client;

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

channel.ExchangeDeclare(exchange: "serialization-exchange",
                        type: "fanout",
                        durable: true,
                        autoDelete: false);

channel.QueueBind(queue: "serialization-queue",
                  exchange: "serialization-exchange",
                  routingKey: String.Empty,
                  arguments: null);


for (int i = 0; i < 100; i++)
{
    var banana = new banana()
    {
        Id = i,
        Author2 = "test",
        Description = "123",
        Name = "456"
    };

    var message = JsonSerializer.Serialize(banana);

    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "serialization-exchange",
                    routingKey: String.Empty,
                    basicProperties: null,
                    body: body);
    Console.WriteLine($"Sent message #{i + 1}: {message}");
}

Console.WriteLine("[END] Press any key to close...");
Console.ReadKey();