using System.Text;
using RabbitMQ.Client;

Console.WriteLine("[START]");

var factory = new ConnectionFactory {
    HostName = "localhost"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "test-queue1",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

channel.ExchangeDeclare(exchange: "test-exchange1",
                        type: "fanout",
                        durable: true,
                        autoDelete: false);

channel.QueueBind(queue: "test-queue1", 
                  exchange: "test-exchange1",
                  routingKey: String.Empty,
                  arguments: null);

const string message = "Our message! :)";

var body = Encoding.UTF8.GetBytes(message);

for(int i = 0; i < 100; i++)
{
    channel.BasicPublish(exchange: "test-exchange1",
                    routingKey: String.Empty,
                    basicProperties: null,
                    body: body);
    Console.WriteLine($"Sent message #{i + 1}: {message}");
}

Console.WriteLine("[END] Press any key to close...");
Console.ReadKey();