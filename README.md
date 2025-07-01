# Proyecto de ejemplo: Envío y recepción de mensajes con RabbitMQ

Este proyecto es una guía básica para entender cómo funcionan las colas (queue), los exchanges y la publicación/recepción de mensajes usando RabbitMQ.

## Instalación de RabbitMQ usando Docker

1. Asegúrate de tener Docker instalado en tu máquina.
2. Ejecuta el siguiente comando para iniciar un contenedor de RabbitMQ con la interfaz de administración habilitada:

   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

3. Accede a la interfaz de administración en: [http://localhost:15672](http://localhost:15672)
   - Usuario: `guest`
   - Contraseña: `guest`

## Conceptos básicos

- **Queue (Cola):** Es donde se almacenan los mensajes hasta que son consumidos por un receptor.
- **Exchange:** Recibe los mensajes de los productores y los dirige a una o varias colas según reglas de enrutamiento.
  - **Fanout Exchange:** Un exchange de tipo `fanout` reenvía todos los mensajes que recibe a todas las colas que están enlazadas a él, sin importar el contenido del mensaje o la clave de enrutamiento.
- **Publicar mensajes:** Es el proceso de enviar mensajes a un exchange.
- **Recibir mensajes:** Es el proceso de consumir mensajes desde una cola.
- **ACK (Acknowledgement):** Es una confirmación que envía el consumidor a RabbitMQ para indicar que el mensaje fue recibido y procesado correctamente. Si no se envía el `ack`, el mensaje puede ser reenviado a otro consumidor.

## Ejecución del proyecto

1. Asegúrate de que RabbitMQ esté corriendo (ver sección de instalación).
2. Ejecuta el emisor (sender) para publicar mensajes.
3. Ejecuta el receptor (receiver) para consumir mensajes de la cola.

## Estructura del proyecto

- `sender` : Proyecto para enviar mensajes.
- `receiver`: Proyecto para recibir mensajes.


## Ejemplo de código

### Enviar mensajes (Sender)

```csharp
// Program.cs del proyecto Send
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: "fanout");

string message = "Hola desde el sender!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "logs",
                     routingKey: "",
                     basicProperties: null,
                     body: body);

Console.WriteLine($"[x] Enviado: {message}");
```

### Recibir mensajes (Receiver)

```csharp
// Program.cs del proyecto Recibe
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: "fanout");
var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName,
                  exchange: "logs",
                  routingKey: "");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Recibido: {message}");
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // ACK manual
};

channel.BasicConsume(queue: queueName,
                     autoAck: false, // Importante para usar ACK manual
                     consumer: consumer);

Console.WriteLine("Presiona [enter] para salir.");
Console.ReadLine();
```

## Recursos útiles

- [Documentación oficial de RabbitMQ](https://www.rabbitmq.com/documentation.html)
- [RabbitMQ Tutorials](https://www.rabbitmq.com/getstarted.html)
