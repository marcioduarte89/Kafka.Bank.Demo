using Confluent.Kafka;
using Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var bootstrapperServer = _configuration.GetValue<string>("BootstrapperServer");
            var topicaName = _configuration.GetValue<string>("TopicName");
            var consumerGroupId = _configuration.GetValue<string>("ConsumerGroupId");

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapperServer,
                GroupId = consumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };

            using (var consumer = new ConsumerBuilder<int, string>(config).Build())
            {
                consumer.Subscribe(topicaName);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumerResult = consumer.Consume(stoppingToken);

                        if (consumerResult == null || consumerResult.IsPartitionEOF)
                        {
                            continue;
                        }

                        /*
                         * Consume messages here, retry if failure, and commit if success
                        */
                        var transaction = JsonSerializer.Deserialize<Transaction>(consumerResult.Message.Value);

                        Console.WriteLine($"Consumed transaction with Id {transaction.Id}, and value {transaction.Value} at: {consumerResult.TopicPartitionOffset}.");

                        // Temporarily stores the offset so we can then commit it.
                        consumer.StoreOffset(consumerResult);

                        // Commits the partition offset
                        consumer.Commit(consumerResult);

                        var committedPartitionOffsets = consumer.Committed(new List<TopicPartition>
                        {
                            consumerResult.TopicPartitionOffset.TopicPartition
                        },
                        TimeSpan.FromSeconds(10));

                        foreach (var committedPartitionOffset in committedPartitionOffsets)
                        {
                            Console.WriteLine($"Committed partition offset {committedPartitionOffset.Offset.Value}.");
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
        }
    }
}
