using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

string queueName = "hello";
try
{
    var factory = new ConnectionFactory { HostName = "localhost" };
    
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

   await channel.QueueDeclareAsync(queueName, true, false, false, null);
    await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += (model, ea) =>
    {
        byte[] body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] {message}");
        return Task.CompletedTask;
    };

    Console.ReadLine();
    await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

Console.ReadLine();