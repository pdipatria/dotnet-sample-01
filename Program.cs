using my_first_service;

using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        var app = builder.Build();
// #if DEBUG
        foreach (var c in app.Configuration.AsEnumerable())
        {
            Console.WriteLine($"CONFIG: Key: {c.Key} Value: {c.Value}");
        }
// #endif

#pragma warning disable CS8601 // Possible null reference assignment.
        var factory = new ConnectionFactory
        {
            HostName = builder.Configuration["RabbitMQ:HostName"] ?? "rabbitmq",
            UserName = builder.Configuration["RabbitMQ:UserName"] ?? "admin",
            Password = builder.Configuration["RabbitMQ:Password"]
        };

        app.MapGet("/", () => "Hello World!");

        app.MapGet("/greet/{name}", (string name) => new Greeting { Name = name, Message = $"Hello {name}" });

        app.MapGet("/greetings", (AppDbContext appDbContext) =>
        {
            return appDbContext.Greeting.ToList();
        });

        app.MapPost("/greetings", (AppDbContext db, GreetingReq greeting) =>
        {
            db.Greeting.Add(new Greeting { Name = greeting.Name, Message = greeting.Message });
            db.SaveChanges();
        });

        app.MapPost("/greetingsWithNotify", async (GreetingReq request, AppDbContext db) =>
        {
            var greeting = new Greeting
            {
                Name = request.Name,
                Message = request.Message
            };

            db.Greeting.Add(greeting);
            await db.SaveChangesAsync();

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "greetings-created", durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(greeting));
            await channel.BasicPublishAsync(exchange: "", routingKey: "greetings-created", body: body);

            return Results.Ok(greeting);
        });


        app.Run();
    }
}

#if DEBUG

#endif

