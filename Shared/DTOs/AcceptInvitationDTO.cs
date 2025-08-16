using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class AcceptInvitationDTO
{
    [Required]
    public required string InvitationSataus { get; set; }
}
