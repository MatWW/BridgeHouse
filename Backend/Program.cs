using Backend.Hubs;
using Backend.Services;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Backend.Repositories;
using Backend.ExceptionHandlers;
using Microsoft.AspNetCore.Identity;
using Backend.Data.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:7087")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidateAudience = true,
    ValidAudience = builder.Configuration["Jwt:Audience"],
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = tokenValidationParameters;
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Request.Cookies.TryGetValue("access-token", out string? token);
            context.Token = token;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:5002")
);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBridgeTablesService, BridgeTablesService>();
builder.Services.AddScoped<IRedisBridgeTableRepository, RedisBridgeTableRepository>();
builder.Services.AddScoped<IUserRepository,  UserRepository>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IRedisGameStateRepository,  RedisGameStateRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IBiddingService, BiddingService>();
builder.Services.AddScoped<IPlayingService, PlayingService>();
builder.Services.AddScoped<IRedisPlayerStateRepository, RedisPlayerStateRepository>();
builder.Services.AddScoped<IUserStateService, UserStateService>();
builder.Services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
builder.Services.AddScoped<IGameHistoryService, GameHistoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>(); ;
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSignalR();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton<Random>();

builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    Console.WriteLine("Request path: " + context.Request.Path);
    await next();
});

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<BridgeHub>("/gameHub");

app.Run();
