using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyMassNotificationService.Application.Interfaces;
using MyMassNotificationService.Application.Options;
using MyMassNotificationService.Application.ProjectSettings;
using MyMassNotificationService.Application.Services;
using MyMassNotificationService.Infrastructure.Data;
using MyMassNotificationService.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

builder.Services.PostConfigure<KafkaOptions>(options =>
{
    if (string.IsNullOrEmpty(options.BootstrapServers))
        options.BootstrapServers = string.Join(",", new []{ "localhost:9092", "localhost:9093", "localhost:9094"});
});

builder.Services.AddHostedService<OutboxProcessor>();
builder.Services.AddHostedService<OutboxCleanupService>();
builder.Services.AddHttpClient();

if (env == ASPNETCORE_ENVIRONMENTS.Development.ToString())
{

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL")));
}
if (env == ASPNETCORE_ENVIRONMENTS.Testing.ToString())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase(Guid.NewGuid().ToString())
        .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
    });
}

builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("EmailOptions"));
builder.Services.Configure<WebAPIOutboxOptions>(builder.Configuration.GetSection("WebApiOutboxBaseUrl"));

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<IEmailServiceFactory, EmailServiceFactory>();
builder.Services.AddSingleton<KafkaNotificationConsumer>();

builder.Services.AddScoped<IOutboxService, OutboxService>();
builder.Services.AddSingleton<KafkaNotificationProducer>();



builder.Services.AddHttpContextAccessor();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
