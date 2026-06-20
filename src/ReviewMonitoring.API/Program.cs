
using ReviewMonitoring.AI.Extensions;
using ReviewMonitoring.Application.Extensions;
using ReviewMonitoring.Infrastructure.Postgres;
using ReviewMonitoring.Infrastructure.Postgres.Extensions;
using ReviewMonitoring.Infrastructure.Redis.Extensions;
using ReviewMonitoring.Ingestion.Browser.Extensions;
using ReviewMonitoring.Ingestion.Extensions;
using ReviewMonitoring.Ingestion.Http.Extensions;
using ReviewMonitoring.Processing.Extensions;
using ReviewMonitoring.Shared;
using ReviewMonitoring.Shared.Consts;

namespace ReviewMonitoring.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Инфрсатруктура
        builder.Services.AddApplication();

        builder.Services.AddPostgresInfrastructure(builder.Configuration);
        builder.Services.AddRedisInfrastructure(builder.Configuration);

        builder.Services.AddBrowserIngestion();
        builder.Services.AddHttpIngestion();
        builder.Services.AddIngestion(builder.Configuration);

        builder.Services.AddAi(builder.Configuration);
        builder.Services.AddProcessing();

        var app = builder.Build();

        //TODO: сделать static bool Demomode в сшаред и отуда тянуть шоб не парсить в bool постоянно из конфига
        if (bool.TryParse(app.Configuration[ConstsShared.Keys.DemoMode], out var demo) && demo)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            db.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        ILoggerFactory? loggerFactory = app.Services.GetService<ILoggerFactory>();
        if (loggerFactory != null)
        {
            AppLog.LoggerFactory = loggerFactory;
        }

        app.Run();
    }
}
