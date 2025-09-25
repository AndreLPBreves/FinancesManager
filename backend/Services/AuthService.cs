using backend.Common;
using backend.Data;
using backend.Extensions;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AuthService(
        AppDbContext dbContext,
        PasswordHasherService passwordHasher,
        JwtService jwtService,
        IConfiguration configuration
    )
    {
        public async Task<Result<User>> LoginUserAsync(string email, string password)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(user =>
                user.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)
            );
            if (dbUser == null || !dbUser.VerifyPassword(password, passwordHasher))
            {
                return new Result<User>(StatusCode.InvalidCredentials, null);
            }

            var newSession = Session.Create(
                dbUser,
                double.Parse(configuration["Jwt:Authentication:ExpirationMinutes"]!),
                jwtService
            );

            await dbContext.Sessions.AddAsync(newSession);
            await dbContext.SaveChangesAsync();

            return new Result<User>(StatusCode.OK, dbUser);
        }
    }
}
