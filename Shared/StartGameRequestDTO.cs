namespace Shared;

public class StartGameRequestDTO
{
    public long tableId { get; set; }
    public List<Player> players { get; set; } = [];
}
