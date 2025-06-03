using Backend.Hubs;
using Backend.Services;
using Backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Backend.Repositories;
using Backend.Models;
using Backend.ExceptionHandlers;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddSignalR();

builder.Services.AddAuthorization();

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});


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


builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IBridgeTablesService, BridgeTablesService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:5002")
);

builder.Services.AddScoped<IRedisBridgeTableRepository, RedisBridgeTableRepository>();
builder.Services.AddScoped<IUserRepository,  UserRepository>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IRedisGameStateRepository,  RedisGameStateRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IBiddingService, BiddingService>();
builder.Services.AddScoped<IPlayingService, PlayingService>();
builder.Services.AddScoped<IRedisPlayerStateRepository, RedisPlayerStateRepository>();
builder.Services.AddScoped<IPlayerStateService, PlayerStateService>();
builder.Services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
builder.Services.AddScoped<IGameHistoryService, GameHistoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSignalR();


builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<UserNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BridgeTableNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BridgeTableOwnershipExceptionHandler>();
builder.Services.AddExceptionHandler<AddPlayerConflictExceptionHandler>();
builder.Services.AddExceptionHandler<PlayerNotFoundAtBridgeTableExceptionHandler>();
builder.Services.AddExceptionHandler<PlayersListNotValidExceptionHandler>();
builder.Services.AddExceptionHandler<GameAlreadyStartedExceptionHandler>();
builder.Services.AddExceptionHandler<GameNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedAccessExceptionHandler>();
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

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.MapHub<BridgeHub>("/gameHub");



app.Use(async (context, next) =>
{
    Console.WriteLine("Request path: " + context.Request.Path);
    await next();
});

app.Run();
