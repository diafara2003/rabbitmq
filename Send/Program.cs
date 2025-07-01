using RabbitMQ.Client;

string queueName = "hello";
try
{
    Console.WriteLine("Connecting to RabbitMQ...");
    var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();


    // await channel.QueueDeclareAsync(queueName, true, false, false, null);
    // await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty);
    string opcion = Console.ReadLine() ?? "1";
    const string message = "Hello World!";
    var body = System.Text.Encoding.UTF8.GetBytes(message);

    if (opcion == "1")
    {
        Console.WriteLine("Sending message Fanout...");
        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
        await channel.BasicPublishAsync(exchange: "logs", routingKey: String.Empty, body: body);
    }
    else
    {
        Console.WriteLine("Sending message Direct...");
        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Direct);
        await channel.BasicPublishAsync(exchange: "logs", routingKey: "info", body: body);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
}

Console.WriteLine("sending message to RabbitMQ...");
Console.ReadLine();