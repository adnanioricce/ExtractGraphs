namespace Lib

module Env =
    open System
    let private get name = Environment.GetEnvironmentVariable(name)
    let RABBITMQ_DEFAULT_QUEUE = get "RABBITMQ_DEFAULT_QUEUE"
    let RABBITMQ_DEFAULT_USER = get "RABBITMQ_DEFAULT_USER"
    let RABBITMQ_DEFAULT_PASS = get "RABBITMQ_DEFAULT_PASS"
    let RABBITMQ_CONNECTION_STR = get "RABBITMQ_CONNECTION_STR"
