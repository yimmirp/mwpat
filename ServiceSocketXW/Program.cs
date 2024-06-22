using Microsoft.EntityFrameworkCore;
using Serilog.Filters;
using Serilog;
using ServiceSocketXW;
using ServiceSocketXW.Data;
using ServiceSocketXW.Core.Socket;
using ServiceSocketXW.Core.Socket.Interfaces;
using ServiceSocketXW.Core.EF;
using ServiceSocketXW.Core.EF.Interfaces;
using ServiceSocketXW.Core.Handler.Interfaces;
using ServiceSocketXW.Core.Handler;

IHost host = Host.CreateDefaultBuilder(args)
.UseWindowsService()
.ConfigureServices((hostContext, services) =>
    {

        var logFile = Path.Combine(hostContext.Configuration["ENV:LOGPATH"] ?? "", $"MWPATLOG-.txt");

        services.AddLogging(loggingBuilder => {
            loggingBuilder.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day).CreateLogger());
        });

        services.AddDbContext<DbPagosContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("DB_PAGOS")));
        services.AddSingleton<IConnectionSocket, ConnectionSocket>();
        services.AddScoped<IPagoService, PagoService>();
        services.AddSingleton<IHandlerServer, HandlerServer>();
        services.AddSingleton(hostContext.Configuration);


        services.AddSingleton<ISocketService, SocketService>(provider =>
        {
            return new SocketService(
             provider.GetRequiredService<IConnectionSocket>(),
             provider.GetRequiredService<IHandlerServer>(),
             provider.GetRequiredService<IConfiguration>(),
             provider.GetRequiredService<ILogger<SocketService>>()
            );
        });

        services.AddSingleton(hostContext.Configuration);

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();