using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Service;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer.Helper
{
    public class RabbitMQConsumer : BackgroundService
    {

        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumer(IConnectionFactory connectionFactory, IServiceProvider serviceProvider) 
        {
            _connectionFactory = connectionFactory;
            _serviceProvider = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = _connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "emailQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var parts = message.Split('|');

                if (parts.Length == 3)
                {
                    string toEmail = parts[0];
                    string subject = parts[1];
                    string bodyContent = parts[2];

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                        emailService.SendEmail(toEmail, subject, bodyContent);
                    }
                }

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: "emailQueue",
                                 autoAck: false,
                                 consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
