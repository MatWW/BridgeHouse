using System.ComponentModel.DataAnnotations;
using Backend.Enums;

namespace Backend.Data.Models;

public class UserGames
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    [Required] 
    public int GameId { get; set; }
    [Required]
    public Position UserPosition { get; set; }
}
