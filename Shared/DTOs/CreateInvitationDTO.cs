using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class CreateInvitationDTO
{
    [Required]
    public required string UserId { get; set; }
    [Required]
    public Position? Position { get; set; }
}
