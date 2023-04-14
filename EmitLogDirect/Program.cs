using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(
    exchange: "direct_logs",
    type: ExchangeType.Direct);

var severity = (args.Length > 0) ? args[0] : "info"; // param from command: dotnet run param1

var message = (args.Length > 1) // param from command: dotnet run param1 [param2 param3]...
              ? string.Join(" ", args.Skip(1).ToArray())
              : "Hello World!";

var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "direct_logs",
                     routingKey: severity, // routing key entered by the user: info, warning, error
                     basicProperties: null,
                     body: body);

Console.WriteLine($" [x] Sent '{severity}':'{message}'");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();