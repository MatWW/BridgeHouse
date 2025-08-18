using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateBridgeTableRequestDTO
{
    [Range(1, 8, ErrorMessage = "Number of deals must be between 1 and 8.")]
    public int NumberOfDeals { get; set; }
}
