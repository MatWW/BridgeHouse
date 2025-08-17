using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Data.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Nickname { get; set; }
    [Required]
    public string PasswordHash { get; set; }
}
