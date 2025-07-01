using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var queueName = "hello";
var exchange = "logs";
try
{
    var factory = new ConnectionFactory { HostName = "localhost" };

    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    //await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);

    await channel.QueueDeclareAsync(queueName, true, false, false, null);
    await channel.QueueBindAsync(queue: queueName, exchange: exchange, routingKey: string.Empty);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] {message}");
        return Task.CompletedTask;
    };

   
    await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

Console.ReadLine();