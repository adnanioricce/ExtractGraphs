namespace Worker

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Lib.Interfaces.RabbitQueue

module Program =
    open RabbitMQ.Client
    open Lib

    [<EntryPoint>]
    let main args =
        let builder = Host.CreateApplicationBuilder(args)
        builder.Services.AddHostedService<ExtractGraphWorker>() |> ignore
        // builder.Services.AddSingleton<RabbitMQClient>(
        //     fun sp -> 
        //         let config:RabbitMQDeps = {
        //             ConnectionFactory = ConnectionFactory()
        //             QueueName = Env.RABBITMQ_DEFAULT_QUEUE
        //         }
        //         RabbitMQClient(config)) |> ignore
        builder.Build().Run()

        0 // exit code