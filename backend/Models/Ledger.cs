using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Ledger
    {
        public const int nameMaxLength = 50;
        public Guid Id { get; init; } = Guid.CreateVersion7();

        [Required(ErrorMessage = "OwnerId can't be null")]
        public required Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        [Required(ErrorMessage = "Name can't be null")]
        [StringLength(nameMaxLength, ErrorMessage = "Max characters allowed is {1}")]
        public required string Name { get; set; } = string.Empty;

        public ICollection<LedgerAllowedUser> AllowedUsers { get; set; } = [];
    }
}
