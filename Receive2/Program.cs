﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var routingKey = "info";
var exchange = "logs";
var queueName = "hello2";

try
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    await using var connection = await factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    //await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Direct);
    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
        arguments: null);
    await channel.QueueBindAsync(queue: queueName, exchange: exchange, routingKey: routingKey);

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync +=  (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] {message}");
        return Task.CompletedTask;
    };

    await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.ReadLine();