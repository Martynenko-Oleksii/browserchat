using BrowserChat.Security.Core.Data;
using BrowserChat.Security.Core.Entities;
using BrowserChat.Security.Core.JWTLogic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
bool isProduction = builder.Environment.IsProduction();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Identity Db Context */
/**************/
builder.Services.AddDbContext<SecurityContext>(opt =>
{
    if (isProduction)
    {
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConn"));
    }
    else
    {
        opt.UseInMemoryDatabase("InMem");
    }
});

var secBuilder = builder.Services.AddIdentityCore<User>();
var identityBuilder = new IdentityBuilder(secBuilder.UserType, secBuilder.Services);
identityBuilder.AddEntityFrameworkStores<SecurityContext>();
identityBuilder.AddSignInManager<SignInManager<User>>();
builder.Services.AddSingleton<ISystemClock, SystemClock>();
/**************/

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWTKey"))),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddSingleton<IJWTGenerator, JWTGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await Persistence.PrepPopulation(app, isProduction);

app.Run();
