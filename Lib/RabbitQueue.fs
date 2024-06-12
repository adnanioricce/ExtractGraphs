namespace Lib.Interfaces

module Queue =
    open System
    type GraphSource =
    | Url of string
    | Html of string
    type QueueMessage<'a> = {
        Body: 'a
        CreatedAt: DateTimeOffset
    } 
    type ExtractGraphMessage = QueueMessage<GraphSource>
    // Definition of a message with information to extract graph from pages
    type ExtractGraphMessageHandler = (ExtractGraphMessage -> Async<unit>)    
    // let enqueue enqueue : ExtractGraphMessageHandler = 
    //     fun req -> async {            
    //         return ()
    //     }        
module RabbitQueue =
    open Queue
    open System.Text
    open RabbitMQ.Client
    // open Microsoft.Extensions.Logging
    open RabbitMQ.Client.Events
    type RabbitMQDeps = {        
        ConnectionFactory: ConnectionFactory
        QueueName: string

    }
    type RabbitMQClient(deps: RabbitMQDeps) =
        let connection = deps.ConnectionFactory.CreateConnection()
        let channel = connection.CreateModel()
        // let logger = LoggerFactory.Create(fun builder -> builder.AddConsole() |> ignore)        

        do            
            // Declaring a queue
            // TODO: this should be done on each message, or just once?
            channel.QueueDeclare(
                queue = deps.QueueName,
                durable = false,
                exclusive = false,
                autoDelete = false,
                arguments = null) |> ignore

        member this.Publish(msg:QueueMessage<'a>) = async {            
            let msgBody = Json.JsonSerializer.Serialize(msg)
            let msgData = Encoding.UTF8.GetBytes(msgBody)
            channel.BasicPublish(exchange = "",
                    routingKey = deps.QueueName,
                    basicProperties = null,
                    body = msgData)
            //TODO: Log information
            // let queue = System.Collections.Generic.Queue<string>()            
        }
        //Note: Handle this in a worker process, not here
        member this.Dequeue<'a>(handler: 'a -> Async<unit>) =
            let consumer: EventingBasicConsumer = EventingBasicConsumer(channel)
            consumer.Received.AddHandler(
                fun model ea -> 
                    let body = ea.Body.ToArray()
                    let message = 
                        Encoding.UTF8.GetString(body) 
                        |> (fun json -> Json.JsonSerializer.Deserialize<'a>(json))
                    (handler message) |> Async.RunSynchronously
                    //TODO: Log Info
                    // Console.WriteLine($" [x] Received {message}")
                )
            channel.BasicConsume(
                    queue = deps.QueueName,
                    autoAck = true,
                    consumer = consumer) |> ignore
                        
            
    
    

