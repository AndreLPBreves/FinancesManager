using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class LedgerAccessLevel
    {
        public const int descriptionMaxLengh = 30;

        [Key]
        public Guid Id { get; init; } = Guid.CreateVersion7();

        [Required(ErrorMessage = "Description can't be null")]
        [StringLength(descriptionMaxLengh, ErrorMessage = "Max characters allowed is {1}")]
        public required string Description { get; set; } = string.Empty;

        public ICollection<LedgerAllowedUser> AllowedUsers { get; set; } = [];
    }
}
