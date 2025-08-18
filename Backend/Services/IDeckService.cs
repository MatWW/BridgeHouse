using Backend.Models;

namespace Backend.Services;

public interface IDeckService
{
    public Dictionary<string, List<Card>> DealCards(List<string> playersIds);
}
