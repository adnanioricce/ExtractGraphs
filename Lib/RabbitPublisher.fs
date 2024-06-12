namespace Lib.Interfaces

module RabbitPublisher =
    open Lib
    open System
    open RabbitMQ.Client
    open Queue
    let a = "nothing"
    
    let private client = RabbitQueue.RabbitMQClient({
        ConnectionFactory = ConnectionFactory(UserName = Env.RABBITMQ_DEFAULT_USER,Password = Env.RABBITMQ_DEFAULT_PASS)
        QueueName = Env.RABBITMQ_DEFAULT_QUEUE
    })
    let publish<'a> (messageBody:'a) = async {
        let msg: QueueMessage<'a> = { Body = messageBody; CreatedAt = DateTimeOffset.UtcNow }
        do! client.Publish(msg)
        
    }
    let subscribe (handler:'a -> Async<unit>) = client.Dequeue(handler)