using System.ComponentModel.DataAnnotations;
using backend.DTOs;
using backend.Services;

namespace backend.Models
{
    public class User
    {
        public const int firstNameMaxLength = 25;
        public const int lastNameMaxLength = 25;
        public const int emailMaxLength = 50;

        [Key]
        public Guid Id { get; init; } = Guid.CreateVersion7();

        [Required(ErrorMessage = "First name can't be null")]
        [StringLength(firstNameMaxLength, ErrorMessage = "Max characters allowed is {1}")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name can't be null")]
        [StringLength(lastNameMaxLength, ErrorMessage = "Max characters allowed is {1}")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email can't be null")]
        [StringLength(emailMaxLength, ErrorMessage = "Max characters allowed is {1}")]
        public required string Email { get; set; }
        public bool IsConfirmed { get; set; } = false;

        [Required(ErrorMessage = "PasswordHash can't be null")]
        public string PasswordHash { get; private set; } = string.Empty;

        [Required(ErrorMessage = "PasswordSalt can't be null")]
        public string PasswordSalt { get; private set; } = string.Empty;

        public ICollection<Ledger> Ledgers { get; set; } = [];
        public ICollection<LedgerAllowedUser> AccessibleLedgers { get; set; } = [];
        public ICollection<Session> Sessions { get; set; } = [];

        public void SetPassword(string password, PasswordHasherService passwordHasher)
        {
            this.PasswordSalt = passwordHasher.GenerateSalt();
            this.PasswordHash = passwordHasher.CalculateHash(password, this.PasswordSalt);
        }

        public static User FromUserRegistrationDTO(UserRegistrationDTO userRegistrationDTO)
        {
            return new User
            {
                FirstName = userRegistrationDTO.FirstName,
                LastName = userRegistrationDTO.LastName,
                Email = userRegistrationDTO.Email,
            };
        }
    }
}
