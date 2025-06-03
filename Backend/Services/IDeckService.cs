using Shared.Models;

namespace Backend.Services;

public interface IDeckService
{
    public Dictionary<string, List<Card>> DealCards(List<string> playersIds);
}
