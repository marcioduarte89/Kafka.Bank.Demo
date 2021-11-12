using Confluent.Kafka;
using Messages;
using Microsoft.Extensions.Configuration;
using Producer.API.Services.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Producer.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IProducer<int, string> _producer;
        private readonly IConfiguration _configuration;

        public TransactionService(IProducer<int, string> producer, IConfiguration configuration)
        {
            _producer = producer;
            _configuration = configuration;
        }

        public async Task CreateTransaction(Transaction transaction, CancellationToken cancellationToken)
        {
            var topicName = _configuration.GetValue<string>("TopicName");

            var message = new Message<int, string> 
            {
                Key = transaction.UserId, 
                Value = JsonSerializer.Serialize(transaction) 
            };

            var deliveryResult = await _producer.ProduceAsync(topicName, message, cancellationToken);
            Console.WriteLine($"Message written to partition { deliveryResult.Partition.Value } with current offset {deliveryResult.Offset.Value}");
        }
    }
}
