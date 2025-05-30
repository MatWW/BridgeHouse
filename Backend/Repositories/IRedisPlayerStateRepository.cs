using Shared.Enums;

namespace Backend.Repositories;

public interface IRedisPlayerStateRepository
{
    Task<long?> GetTableIdOfPlayerInviteAsync(string playerId); 
    Task<Position?> GetPositionOfPlayerInviteAsync(string playerId);
    Task<long?> GetTableIdOfPlayerAsync(string playerId);
    Task<long?> GetGameIdOfPlayerAsync(string playerId);

    Task SaveInformationAboutPlayerBeingInvitedToTableAsync(string playerId, long tableId, Position position);
    Task SaveInformationAboutPlayerBeingPartOfTableAsync(string playerId, long tableId);
    Task SaveInformationAboutPlayerBeingInGameAsync(string playerId, long gameId);

    Task DeleteInformationAboutPlayerBeingInvitedToTableAsync(string playerId);
    Task DeleteInformationAboutPlayerBeingPartOfTableAsync(string playerId);
    Task DeleteInformationAboutPlayerBeingInGameAsync(string playerId);
}
