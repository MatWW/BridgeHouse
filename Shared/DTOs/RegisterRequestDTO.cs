using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class RegisterRequestDTO
{
    [Required]
    public required string Email {  get; set; }
    [Required]
    public required string Nickname { get; set; }
    [Required]
    public required string Password { get; set; } 
}
