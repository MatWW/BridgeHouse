using Shared;

namespace Backend.Services;

public interface IPlayingService
{
    Task PlayCardAsync(long gameId, CardPlayAction cardPlayAction);
}
