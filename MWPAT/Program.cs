using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManage.Data;
using SiteManage.Services.EF;
using SiteManage.Services.Handler;
using SiteManage.Services.Handler.Interfaces; 
using SiteManage.Services.SocketUitils;
using SiteManage.Services.SocketUitils.Interfaces;
using SiteManage.Services.TaskBackground;
using Serilog;
using Serilog.Filters;
var builder = WebApplication.CreateBuilder(args);
var logFile = Path.Combine(builder.Configuration["ENV:LOGPATH"] ?? "", $"MWPATLOG-.txt");

// Add services to the container.

builder.Services.AddLogging(loggingBuilder =>{ 
    loggingBuilder.AddSerilog(new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.File(logFile,rollingInterval:RollingInterval.Day).CreateLogger());
});
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<DbPagosContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DB_PAGOS")));
builder.Services.AddSingleton<IConnectionSocket, ConnectionSocket>();
builder.Services.AddScoped<IPagoService, PagosService>();
builder.Services.AddSingleton<IHandlerServer, HandlerServer>();
builder.Services.AddSingleton<IHandlerClient, HandlerClient>();
builder.Services.AddSingleton(builder.Configuration);


builder.Services.AddSingleton<ISocketService, SocketService>(provider => {
    return new SocketService(
     provider.GetRequiredService<IConnectionSocket>(),
     provider.GetRequiredService<IHandlerServer>(),
     provider.GetRequiredService<IHandlerClient>(),
     provider.GetRequiredService<IConfiguration>(),
     provider.GetRequiredService<ILogger<SocketService>>()
    );
});


builder.Services.AddHostedService<OnInitTask>();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
