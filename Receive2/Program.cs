using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


var queueName = "hello2";
var exchange = "logs";
try
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    await channel.QueueBindAsync(queue: queueName, exchange: exchange, routingKey: string.Empty);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += async (model, ea) =>
    {
        byte[] body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received: {message}");
        // Simulate some processing
        await Task.Delay(1000);
        Console.WriteLine(" [x] Done");
    };

    await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.ReadLine();