[![Build Status](https://github.com/arjangeertsema/zeebe-client-csharp-bootstrap/actions/workflows/ci.yml/badge.svg)](https://github.com/arjangeertsema/zeebe-client-csharp-bootstrap/actions/workflows/ci.yml)
[![](https://img.shields.io/nuget/v/zb-client-bootstrap.svg)](https://www.nuget.org/packages/zb-client-bootstrap/) 
[![](https://img.shields.io/nuget/dt/zb-client-bootstrap)](https://www.nuget.org/stats/packages/zb-client-bootstrap?groupby=Version) 
[![](https://img.shields.io/github/license/arjangeertsema/zeebe-client-csharp-bootstrap.svg)](https://www.apache.org/licenses/LICENSE-2.0) 
[![Total alerts](https://img.shields.io/lgtm/alerts/g/arjangeertsema/zeebe-client-csharp-bootstrap.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/zeebe-io/zb-csharp-client/alerts/)
[![](https://img.shields.io/badge/Lifecycle-Proof%20of%20Concept-blueviolet)](https://github.com/Camunda-Community-Hub/community/blob/main/extension-lifecycle.md#proof-of-concept-)

# Boostrap extension for the C# Zeebe client

This project is an extension of the [C# Zeebe client project](https://github.com/camunda-community-hub/zeebe-client-csharp). Zeebe Job handlers are automaticly recognized and boostrapped via a [.Net HostedService](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice).

## Requirements

* .Net 5.0
* latest [C# Zeebe client release](https://www.nuget.org/packages/zb-client/)
* latest [Zeebe release](https://github.com/zeebe-io/zeebe/releases/)

## How to use

The Zeebe C# client boostrap extensions is available via nuget (https://www.nuget.org/packages/zb-client-bootstrap/).

See [examples](./examples) for more information.

## Quick start

All classes which implement `IJobHandler<T>` or `IAsyncJobHandler<T>` are automaticly found and boostrapped when you register this boostrap project with the `IServiceCollection` extension method `BoostrapZeebe`.

The `BoostrapZeebe` method has two parameters:

1. `ZeebeBootstrapOptions` via [Configuration, Action delegate or both](https://docs.microsoft.com/en-us/dotnet/core/extensions/options-library-authors).
1. An array with assembly filters, only assemblies which start with one of the filters will be scanned for job handlers.

### Appsettings configuration

```csharp
ConfigureServices((hostContext, services) => {
    services.BootstrapZeebe(
        hostContext.Configuration.GetSection("ZeebeBootstrap"),
        "SimpleExample"
    );
})
```

**appsettings.json**

```json
{
    "ZeebeBootstrap": {
        "Client": {                
            "GatewayAddress": "120.0.0.1:26500"
        },
        "Worker": {
            "MaxJobsActive": 1,
            "TimeoutInMilliseconds": 500,
            "PollIntervalInMilliseconds": 10000,
            "PollingTimeoutInMilliseconds": 30000
        }
    }
}
```

### Delegate configuration

```csharp
ConfigureServices((hostContext, services) => {
    services.BootstrapZeebe(
        options => { 
            options => { 
                options.Client = new ClientOptions() {
                    GatewayAddress = "127.0.0.1:26500"
                };
                options.Worker = new WorkerOptions() 
                {
                    MaxJobsActive = 1,
                    TimeoutInMilliseconds = 10000,
                    PollIntervalInMilliseconds = 30000,
                    PollingTimeoutInMilliseconds = 1000
                };
            }
        },
        "SimpleExample"
    );
})
```

### Job

The job is an implementation of `AbstractJob`. A job can be configured via optional attributes. Job types must be unique.

```csharp
[JobType("SimpleJobV2")]
[WorkerName("SimpleWorker")]
[MaxJobsActive(2)]
[Timeout(500)]
[PollInterval(10000)]
[PollingTimeout(500)]
class SimpleJob : AbstractJob
{
    public SimpleJob(IJob job) 
        : base(job)
    { }

    class JobKeyIsOddException : AbstractJobException 
    {
        public JobKeyIsOddException() 
            : base("1", "Job key is odd.")
        { }
    }
}

class AnotherSimpleJob : AbstractJob
{
    public AnotherSimpleJob(IJob job) 
        : base(job)
    { }

    class Response 
    {
        public bool Property { get; set; }
    }
}
```

### Job handler

The job handler is an implementation of `IJobHandler<Job, Respone>` or `IAsyncJobHandler<Job, Response>`. A jobhandler can be configured via optional attributes. Job handlers are automaticly added to the DI container, therefore you can use dependency injection inside the job handlers. 

A handled job has three outcomes:

1. The job has been handled without exceptions: this will automaticly result in a `JobCompletedCommand` beeing send to the broker. The `Response` is automaticly serialized and added to the `JobCompletedCommand`.
1. An exception has been thrown while handling the job, the exception implements `AbstractJobException`: this wil automaticly result in a `ThrowErrorCommand` beeing send to the broker;
1. Any other exception will automaticly result in a `FailCommand` beeing send to the broker;

```csharp
[ServiceLifetime(ServiceLifetime.Singleton)]
class SimpleJobHandler : IAsyncJobHandler<SimpleJob>, IAsyncJobHandler<AnotherSimpleJob, AnotherSimpleJob.Response>, 
{
    public Task HandleJob(SimpleJob job, CancellationToken cancellationToken)
    {  
        if(job.Key % 2 == 0)
        {            
            //Outcome 1 with no response:
            return Task.CompletedTask;
        }
        else 
        {
            //Outcome 2:
            return Task.FromException(new SimpleJob.JobKeyIsOddException());
        }
    }

    public Task<AnotherSimpleJob.Response> HandleJob(AnotherSimpleJob job, CancellationToken cancellationToken)
    {  
        if(job.Key % 2 == 0)
        {
            //Outcome 1 with response:
            return Task.FromResult
            (
                new AnotherSimpleJob.Response() 
                {  
                    Property = true
                }
            );            
        }
        else
        {
            //Outcome 3:
            throw new Exception("something unexpected has happened");
        }
    }
}
```

## Conventions

This project uses the following conventions:

1. By default the simple name of the `AbstractJob` implementation is used to match the `Type` which is specified in the BPMN model. This can be overriden by adding the `JobTypeAttribute` to the `AbstractJob` implementation.
1. By default the assembly name which contains the job handler is used as the `Worker name`. This can be overriden by adding the `WorkerNameAttribute` to the `AbstractJob` implementation.
1. By default the job handlers are added to de DI container with a `Transient` service lifetime. This can be overriden by adding the `ServiceLifetimeAttribute` to the job handler.

## How to build

Run `dotnet build Zeebe.Client.Bootstrap.sln`.

## How to test

Run `dotnet test Zeebe.Client.Bootstrap.sln`.