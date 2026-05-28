using eCommerceSolution.OrdersService.Data;
using eCommerceSolution.OrdersService.HttpClients;
using eCommerceSolution.OrdersService.Middlewares;
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

// 1. Grab connection settings from appsettings.json
var mongoConnectionString = builder.Configuration.GetConnectionString("DockerMongoDbConnectionString")
    ?? "mongodb://localhost:27017";
var databaseName = builder.Configuration["DatabaseName"]
    ?? "eCommerceOrders";

// 2. Register your minimal DbContext with the MongoDB provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMongoDB(mongoConnectionString, databaseName));


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
