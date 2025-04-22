using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Frontend.Models.Entities;

public class UserAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_name")]
    [MaxLength(100)]
    public string? UserName { get; set; }

    [Column("password")]
    [MaxLength(100)]
    public string? Password { get; set; }
}

