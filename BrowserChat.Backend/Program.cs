using BrowserChat.Backend.Core.AsyncServices;
using BrowserChat.Backend.Core.Data;
using BrowserChat.Backend.Core.HubConfig;
using BrowserChat.Backend.Core.Util;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var a42 = 42;

var builder = WebApplication.CreateBuilder(args);
bool isProduction = builder.Environment.IsProduction();
ConfigurationHelper.Initialize(builder.Configuration);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Db Context */
/**************/
builder.Services.AddDbContext<BrowserChatDbContext>(opt =>
{
    if (isProduction)
    {
        opt.UseSqlServer(ConfigurationHelper.DbConnection);
    }
    else
    {
        opt.UseInMemoryDatabase("InMem");
    }
});

builder.Services.AddScoped<IBrowserChatRepository, BrowserChatRepository>();
/**************/

/* JWT */
/**************/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ConfigurationHelper.JWTKey)),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
/**************/

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

/* MassTransit RabbitMq */
/**************/
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(ConfigurationHelper.RabbitMQHost, "/");

        cfg.ReceiveEndpoint(BrowserChat.Value.Constant.QueueService.QueueName.BotResponse, e =>
        {
            e.Durable = true;
            e.Consumer<BotResponseConsumer>();
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton(typeof(BotRequestPublisher));
/**************/

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMediatR(typeof(BrowserChat.Backend.Core.Application.PostPublish.PostPublishRequest).Assembly);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(ConfigurationHelper.ClientDomain);
    });
});

builder.Services.AddSignalR(opt =>
{
    opt.EnableDetailedErrors = true;
});

builder.Services.AddSingleton(typeof(HubHelper));

var app = builder.Build();

ServiceCollectionHelper.Initialize(app);
Persistence.PrepPopulation(app, isProduction);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsRule");
app.MapHub<HubBase>("/hub");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
