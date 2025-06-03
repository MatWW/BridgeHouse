using Shared.Models;

namespace Shared.DTOs;

public class StartGameRequestDTO
{
    public long tableId { get; set; }
    public List<Player> players { get; set; } = [];
}
