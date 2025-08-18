using Backend.Repositories;
using Backend.Services;

namespace Backend.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserStateService, UserStateService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IBridgeTablesService, BridgeTablesService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IBiddingService, BiddingService>();
        services.AddScoped<IPlayingService, PlayingService>();
        services.AddScoped<IGameHistoryService, GameHistoryService>();
        services.AddScoped<IDeckService, DeckService>();
        services.AddScoped<IRedisPlayerStateRepository, RedisPlayerStateRepository>();
        services.AddScoped<IRedisBridgeTableRepository, RedisBridgeTableRepository>();
        services.AddScoped<IRedisGameStateRepository, RedisGameStateRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGameHistoryRepository, GameHistoryRepository>();
        return services;
    }
}
