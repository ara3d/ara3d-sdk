# Ara3D.Logging

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.Logging)](https://www.nuget.org/packages/Ara3D.Logging)

Ara3D.Logging is a very simple cross-platform library for logging and reporting. 
It was designed particularly for desktop application. 

## How to use it 

1. Create a logger providing a custom or default log writer 
1. Call log on it. 

Enjoy having more insights into what your code is doing and how long it takes. 

## How to Implement a Log Writer 

A log writer is any class that implements the `ILogWriter` interface:

```csharp
    public interface ILogWriter 
    {
        void Write(LogEntry logEntry);
    }
```

A generic log writer class (`LogWriter`) is provided that can be customized via an action
passed to the constructor. 

A default static instance which writes to the console and the debug log.  

```csharp
    public class LogWriter 
    {
        public LogWriter(Action<TimeSpan, LogEntry> onLogEntry);
        public static ILogWriter Create(Action<string> onLogMessage);
        public static ILogWriter Default;
    }
```
     

## Why no Global Static Logger? 

You may find yourself looking for a static "Log" function that you can call, 
or are tempted to write one yourself. 

**Resist the urge!***
 
Well-structured software does not use global mutable state. 

Your code will be easier to reuse and refactor it you explicitly pass an `ILogger`
to every component that might want to use it. 

If you find that you are passing too many values to each component, consider using an `IJob`
which bundles many commonly related concerns together.

## More than just Logging 

When we start introducing logging into an application other related concerns tend to creep
in, like progress reporting, and cancelation. 

Ara3D.Logging introduces a special interface called `IJob` which combines these 
concerns. 

## Job Management

A job is like a high-level task that has a name, can be canceled, provides status and progress reports, 
an error handler event, and a completion handler event. Jobs can have sub-jobs, and link to the previous job. 
Jobs also have results. 

```csharp
    public interface IJob 
        : ICancelable
        , IProgress
        , ILogger
        , INamed
        , IStatus<JobStatus>
        , IErrorHandler
        , ICompletionHandler
    {
        IReadOnlyList<IJob> SubJobs { get; }
        void Start();
        void Stop();
        void Pause();
        void Resume();
        object Result { get; }
        IJob PreviousJob { get; }
    }
```

## Interfaces

* ICancelable 
* ICompletionHandler
* IErrorReporting
* IJob
* ILogger
* ILogWriter
* IProgress
* IStatus

That's a lot. Luckily you can use `Job` instances which implement most of these interfaces for you.  


## Appendix: Compared to Serilog

The most well-known C# logging framework is called Serilog. 

Serilog is for structured object logging, where you can serialize various 
data objects and control the formatting. 

Serilog has an embedded DSL, and a lot of features, whereas Ara3D.Logging 
is designed to be easily used and extended like any code. 

When you are only logging strings, and time stamps, Ara3D.Logging is an alternative
this is much simpler, easier to customize, and more performant.