using backend.Common;
using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class RegisterService(
        AppDbContext dbContext,
        PasswordHasherService passwordHasherService
    )
    {
        public async Task<Result<User>> NewUser(UserRegistrationDTO userData)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(user =>
                user.Email == userData.Email
            );

            if (dbUser != null)
            {
                return new Result<User>(StatusCode.EmailAlredyRegistered, dbUser);
            }

            if (!userData.Password.Equals(userData.PasswordConfirmation))
            {
                return new Result<User>(StatusCode.PasswordConfirmationMismatch, null);
            }

            var newUser = User.FromUserRegistrationDTO(userData);
            newUser.SetPassword(userData.Password, passwordHasherService);

            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync();

            return new Result<User>(StatusCode.OK, newUser);
        }
    }
}
