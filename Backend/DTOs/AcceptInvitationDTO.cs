using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class AcceptInvitationDTO
{
    [Required]
    public required string InvitationSataus { get; set; }
}
