using System.Text;
using BusinessLayer.Helper;
using BusinessLayer.Interface;
using BusinessLayer.Middleware;
using BusinessLayer.Service;
using HelloGreetingApplication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GreetingDB");
// Add services to the container.
builder.Services.AddDbContext<GreetingDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();

//Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hello Greeting API",
        Version = "v1",
        Description = "API for managing greeting messages"
    });
    // Register Custom Operation Filter
    c.OperationFilter<CustomSwaggerOperations>();
});

builder.Services.AddScoped<IGreetingBL,GreetingBL>();
builder.Services.AddScoped<IGreetingRL, GreetingRL>();
builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();


//Add Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

// Register middleware as a service
builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddSingleton<PasswordHasher>();



var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
