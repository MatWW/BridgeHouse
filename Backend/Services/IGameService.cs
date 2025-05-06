using Shared;

namespace Backend.Services;

public interface IGameService
{
    Task StartGame(long tableId, List<Player> players);
}
