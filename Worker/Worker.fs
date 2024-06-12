namespace Worker

open System
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Lib.Interfaces.RabbitQueue
open Lib.Interfaces.Queue
open Lib.Interfaces
open Lib

type ExtractGraphWorker(logger: ILogger<ExtractGraphWorker>) =
    inherit BackgroundService()
    let workerId = Guid.NewGuid()
    let handlerName = nameof Queue.ExtractGraphMessageHandler
    let extractGraph (saveGraph: Structs.Graph<string> -> Async<unit>):  Queue.ExtractGraphMessageHandler =
        fun (msg:ExtractGraphMessage) -> async {
            logger.LogInformation("[Worker {workerId}] [Handler = {handlerName}] Processing message = {message}",workerId ,handlerName,msg)
            match msg.Body with
            | Url url ->                
                let! graph = Extracter.createLinkGraph url Http.getString
                logger.LogInformation("[Worker {workerId}] [Handler = {handlerName}] extracted {url} with success!",workerId ,handlerName,url)
                do! saveGraph graph
                logger.LogInformation("[Worker {workerId}] [Handler = {handlerName}] saved extracted graph from {url} with success!",workerId, handlerName,url)

                let jobs =
                    graph.Iterate(fun key values ->
                        RabbitPublisher.publish { Body = Url key;CreatedAt = DateTimeOffset.UtcNow })
                logger.LogInformation("[Worker {workerId}] [Handler = {handlerName}] is publishing {extractUrlCount} messages of the inner urls!",workerId,handlerName,jobs |> Seq.length)
                //TODO: Use mailbox processor                
                jobs 
                |> Async.Parallel
                |> Async.RunSynchronously
                |> ignore
            // This seems useless here, because the Url case already gets the html string and handles it.
            | Html content ->
                ()
            return ()
        }
    let fakeSaveGraph (graph:Structs.Graph<string>) = async {
        //TODO:
        return ()
    }
    override _.ExecuteAsync(ct: CancellationToken) =
        task {            
            while not ct.IsCancellationRequested do                
                let handler = extractGraph fakeSaveGraph 
                RabbitPublisher.subscribe handler
                do! Task.Delay(TimeSpan.FromSeconds(10))
        }
