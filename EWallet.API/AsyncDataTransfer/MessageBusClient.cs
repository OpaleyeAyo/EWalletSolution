using EWallet.Utility.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace EWallet.API.AsyncDataTransfer
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<MessageBusClient> _logger;

        public MessageBusClient(IOptionsMonitor<RabbitMQSettings> optionMonitor,
            ILogger<MessageBusClient> logger)
        {
            _rabbitMQSettings = optionMonitor.CurrentValue;
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.Host,
                Port = int.Parse(_rabbitMQSettings.Port)
            };

            try
            {
                _connection = factory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: _rabbitMQSettings.ExchangeString, type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMq_ConnectionShutdown;

                _logger.LogInformation("Connected to message bus.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not connect to message bus: {ex.Message}.");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: _rabbitMQSettings.ExchangeString, routingKey: "",
                basicProperties: null, body: body);

            _logger.LogInformation($"{message} ======> message has been sent.");
        }

        private void RabbitMq_ConnectionShutdown(object sender, ShutdownEventArgs arg)
        {
            _logger.LogInformation("Rabbit MQ connection shutdown.");
        }

        public void Dispose()
        {
            _logger.LogInformation("Message bus disposed.");

            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        public void PubMessage<T>(T model)
        {
            var message = JsonSerializer.Serialize(model);

            if (_connection.IsOpen)
            {
                _logger.LogInformation("Connection open, sending message...");

                SendMessage(message);
            }
            else
            {
                _logger.LogError("Connection closed, not sending message...");
            }
        }
    }
}
