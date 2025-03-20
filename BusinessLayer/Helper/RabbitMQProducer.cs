using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BusinessLayer.Helper
{
    public class RabbitMQProducer
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQProducer(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void PublishEmail(string toEmail, string subject, string body)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "emailQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            string message = $"{toEmail}|{subject}|{body}";
            var bodyBytes = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "emailQueue",
                                 basicProperties: null,
                                 body: bodyBytes);

            Console.WriteLine($"✅ Email task queued for: {toEmail}");
        }
    }
}
