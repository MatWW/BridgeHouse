using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Models;

public class GameHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string GameStateJson { get; set; } = string.Empty;
    [Required]
    public string GameShortInfoJson { get; set; } = string.Empty;
}
