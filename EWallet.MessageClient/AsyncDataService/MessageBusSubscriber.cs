using EWallet.MessageClient.EventProcessing;
using EWallet.Utility.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EWallet.MessageClient.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        //private readonly IEventProcessor _eventProcessor;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly ILogger<MessageBusSubscriber> _logger;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IOptionsMonitor<RabbitMQSettings> optionMonitor,
            IServiceScopeFactory scopeFactory,
            ILogger<MessageBusSubscriber> logger)//,IEventProcessor eventProcessor)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            //_eventProcessor = eventProcessor;
            _rabbitMQSettings = optionMonitor.CurrentValue;

            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.Host,
                    Port = int.Parse(_rabbitMQSettings.Port)
                };

                _connection = factory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: _rabbitMQSettings.ExchangeString, type: ExchangeType.Fanout);

                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName,
                    exchange: _rabbitMQSettings.ExchangeString,
                    routingKey: "");

                _logger.LogInformation("Listening on the message bus.");

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs args)
        {
            _logger.LogInformation("Connection shutdown...");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ModuleHandle, ea) =>
            {
                _logger.LogInformation("Event recieved..");

                var body = ea.Body;

                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _logger.LogInformation($"Notification Message {notificationMessage}...");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var _eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();

                    _eventProcessor.ProcessEvent(notificationMessage);
                }

            };

            try
            {
                _logger.LogInformation("Message consumed...");

                var res = _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

                _logger.LogInformation($"Message consumed...{res}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong {ex.Message}");

            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }
    }
}
