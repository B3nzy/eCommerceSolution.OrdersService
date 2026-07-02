using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Middlewares;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("microservices-baseurl.json", optional: false, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add MediatR services
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Register the UsersMicroserviceHttpClient with the base address from configuration
string? userServiceUrl = builder.Configuration["ServiceUrls:UserService"];
builder.Services.AddHttpClient<UsersMicroserviceHttpClient>(client =>
{
    client.BaseAddress = new Uri(userServiceUrl ?? throw new InvalidOperationException("User Service URL is missing."));
});

// Register the ProductsMicroserviceHttpClient with the base address from configuration
string? productsServiceUrl = builder.Configuration["ServiceUrls:ProductsService"];
builder.Services.AddHttpClient<ProductsMicroserviceHttpClient>(client =>
{
    client.BaseAddress = new Uri(productsServiceUrl ?? throw new InvalidOperationException("Products Service URL is missing."));
});

// 1. Grab connection settings from appsettings.json
var mongoConnectionString = builder.Configuration.GetConnectionString("DockerMongoDbConnectionString")
    ?? "mongodb://localhost:27017";
var databaseName = builder.Configuration["DatabaseName"]
    ?? "eCommerceOrders";

// 2. Register your minimal DbContext with the MongoDB provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, databaseName));



// Register Redis Distributed Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "EComPlatform_"; // Optional: Prefixes your cache keys to avoid collision
});

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["MessageBroker:Host"];
        var username = builder.Configuration["MessageBroker:Username"];
        var password = builder.Configuration["MessageBroker:Password"];

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
