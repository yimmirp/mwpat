using HandlerTramaXW;
using Serilog.Filters;
using Serilog;
using HandlerTramaXW.Services.Handler.Interfaces;
using HandlerTramaXW.Services.Handler;




IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        // Obtener la configuración del host
        var configuration = hostContext.Configuration;

        // Configurar Serilog
        var logFile = Path.Combine(configuration["ENV:LOGPATH"] ?? "", $"MWPATLOG-.txt");

        services.AddLogging(loggingBuilder => {
            loggingBuilder.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day).CreateLogger());
        });
        services.AddHostedService<Worker>();
        services.AddSingleton<IHandlerClient, HandlerClient>();
        // Registrar otros servicios aquí, por ejemplo:
        //services.AddSingleton<IHandlerClient, IHandlerClient>();
    })
    .UseWindowsService()
    .Build();

host.Run();
