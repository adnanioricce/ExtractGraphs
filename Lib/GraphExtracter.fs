namespace Lib

open System
open System.Text.RegularExpressions
open System.Net.Http
open System.Collections.Generic
open Newtonsoft.Json
module Structs = 
    type Graph<'T when 'T : equality> () =
        let adjacencyList = Dictionary<'T, List<'T>>()

        member this.AddVertex(vertex: 'T) =
            if not (adjacencyList.ContainsKey(vertex)) then
                adjacencyList.[vertex] <- List<'T>()

        member this.AddEdge(source: 'T, destination: 'T) =
            if not (adjacencyList.ContainsKey(source)) then
                this.AddVertex(source)
            if not (adjacencyList.ContainsKey(destination)) then
                this.AddVertex(destination)
            adjacencyList.[source].Add(destination)

        member this.Vertices = adjacencyList.Keys
        
        member this.GetAdjacencyList(vertex: 'T) =
            if adjacencyList.ContainsKey(vertex) then
                adjacencyList.[vertex]
            else
                List<'T>()
        member this.Iterate<'a>(func:('T -> List<'T> -> 'a)) =
            adjacencyList |> Seq.map (fun kvp -> func kvp.Key kvp.Value)
        
        member this.ToSerializable() =
            this.Iterate (fun key value -> key,value)
            |> dict
        
        member this.PrintGraph() =
            for kvp in adjacencyList do
                printfn "%A -> %A" kvp.Key kvp.Value
module Http =
   use http:HttpClient = new HttpClient()
   let getString (url:string) = async {
       let! response = http.GetStringAsync(url) |> Async.AwaitTask
       return response
   }
module Parser =
    let extractLinks (html: string) : string list =
        let pattern = @"href\s*=\s*[""']([^""']+)[""']"
        let matches = Regex.Matches(html, pattern)
        [ for m in matches -> m.Groups.[1].Value ]
module Extracter = 
    open Structs
    let createLinkGraph (url: string) (fetchHtml:string -> Async<string>) : Async<Graph<string>> = async {
        let graph = Graph<string>()
        let! html = fetchHtml url
        let links = Parser.extractLinks html
        graph.AddVertex(url)
        for link in links do
            graph.AddEdge(url,link)
            
        return graph
    }
    let getLinksFrom (url : string) = async {
        let! response = Http.getString url
        return response 
        |> Parser.extractLinks
        |> Seq.filter (fun url -> url.StartsWith("http"))
    }
    // let rec extractGraphs (url:string) (fetchHtml: string -> Async<string>) : Async<Graph<string>> = async {
    //     let! graph = createLinkGraph url fetchHtml
        
    // }
    let serializeGraphToJson (graph: Graph<'T>) : string =
        let serializableGraph = graph.ToSerializable()
        JsonConvert.SerializeObject(serializableGraph, Formatting.Indented)

