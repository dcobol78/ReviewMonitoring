
using ReviewMonitoring.Shared;
using ReviewMonitoring.Infrastructure.Postgres.Extensions;
using ReviewMonitoring.Infrastructure.Redis.Extensions;
using ReviewMonitoring.Ingestion.Extensions;
using ReviewMonitoring.Ingestion.Browser.Extensions;
using ReviewMonitoring.Ingestion.Http.Extensions;
using ReviewMonitoring.Processing.Extensions;
using ReviewMonitoring.Application.Extensions;
using ReviewMonitoring.AI.Extensions;

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

        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

//Убрать позже если ненужно будет.
#pragma warning disable CS0168 // Variable is declared but never used
        string? configValStr;
#pragma warning restore CS0168 // Variable is declared but never used

#if !DEBUG
        config_val_str = config.GetSection("ConnectionStrings")["Postgres"];
        if (string.IsNullOrWhiteSpace(config_val_str))
        {
        throw new NullReferenceException("Не удалось считать параметр в appsettings.json ->ConnectionStrings:Postgres");
        }
        AppConfig.PostgresConnection = config_val_str;

        config_val_str = config.GetSection("ConnectionStrings")["Redis"];
        if (string.IsNullOrWhiteSpace(config_val_str))
        {
        throw new NullReferenceException("Не удалось считать параметр в appsettings.json ->ConnectionStrings:Redis");
        }
        AppConfig.Redis = config_val_str;
#else
        //AppConfig.PostgresConnection = ConstsDbDebug.DebugPostgresConnection;
        //AppConfig.RedisConnection = ConstsDbDebug.DebugRedisConnection;
#endif

        app.Run();
    }
}
