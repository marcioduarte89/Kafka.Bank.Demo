# Kafka.Bank.Demo
.NET Kafka Demo for [Kafka learning series](https://outofmemoryexception.hashnode.dev/inside-kafka-cli)


## Overview
This .NET Kafka demo is composed by two main projects:

- Producer.API:
  - .NET Core 5.0 Web API with a Transactions API to simulate the creation of transactions. These transactions are sent to Kafka via Idempotent producer which produces to transaction topic. It uses UserId as the message key (ensuring messages from the same user will always be sent to the same topic partition).

- Consumer:
  - Worker which consumes messages from the transactions Topic. This consumer disables auto commit. Messages are processed and offsets committed afterwards.