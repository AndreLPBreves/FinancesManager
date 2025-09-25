using backend.DTOs;
using backend.Models;
using backend.Services;

namespace backend.Extensions
{
    public static class UserExtensions
    {
        public static bool VerifyPassword(
            this User user,
            string password,
            PasswordHasherService passwordHasher
        )
        {
            return passwordHasher.VerifyPassword(user.PasswordHash, password, user.PasswordSalt);
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
