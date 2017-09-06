# apache.log4net.Extensions.Logging [![Build status](https://ci.appveyor.com/api/projects/status/qw5j3ikk8ol2m5sp?svg=true)](https://ci.appveyor.com/project/joshclark/apache-log4net-Extensions-Logging/history) [![NuGet Version](http://img.shields.io/nuget/v/apache.log4net.Extensions.Logging.svg?style=flat)](https://www.nuget.org/packages/apache.log4net.Extensions.Logging/) 


A log4net provider for [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging), the logging subsystem used by ASP.NET Core.

This package routes ASP.NET log messages through log4net, so you can get information about ASP.NET's internal operations logged to the same log4net appenders as your application events.

### Instructions

**First**, install the _apache.log4net.Extensions.Logging_ [NuGet package](https://www.nuget.org/packages/apache.log4net.Extensions.Logging) into your web or console app. 

```powershell
Install-Package apache.log4net.Extensions.Logging 
```

(_Note:_ You should use version 1.x for .NET Core 1.x projects and version 2.x for .NET Core 2.x projects because of [breaking change](https://github.com/aspnet/Announcements/issues/255) in .NET Core logging API introduced in .NET Core 2.0)

**Next**, create a log4net.config in the root of your project, see [log4net.config](https://logging.apache.org/log4net/release/manual/configuration.html) for examples.  You can also use the [config](https://github.com/joshclark/apache.log4net.Extensions.Logging/blob/master/samples/SampleWebApp/log4net.config) included with the sample app  


**Finally (.NET Core 1.x)**, in your `Startup` class's `Configure()` method, remove the existing logger configuration entries and
call `AddLog4Net()` on the provided `loggerFactory`.

```c#
  public void Configure(IApplicationBuilder app,
                        IHostingEnvironment env,
                        ILoggerFactory loggerfactory,
                        IApplicationLifetime appLifetime)
  {
      loggerfactory.AddLog4Net();
      
```

**Finally (.NET Core 2.x)**, in your `Startup` class's `ConfigureServices()` method, remove the existing logger configuration entries and
call `AddLog4Net()` on the `ILoggerBuilder` in `AddLogging`.

```c#
  public IServiceProvider ConfigureServices(IServiceCollection services)
  {
      services.AddLogging(builder => builder.AddLog4Net());
      
```






