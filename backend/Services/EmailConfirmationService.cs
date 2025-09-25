using backend.Common;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class EmailConfirmationService(AppDbContext dbContext)
    {
        public async Task<Result<EmailConfirmation>> ConfirmEmail(string key)
        {
            if (Guid.TryParse(key, out Guid parsedKey))
            {
                var dbEmailConfirmation = await dbContext
                    .EmailConfirmations.Include(e => e.User)
                    .FirstOrDefaultAsync(e => e.Key.Equals(parsedKey));

                if (dbEmailConfirmation == null)
                {
                    return new Result<EmailConfirmation>(StatusCode.InvalidToken, null);
                }

                if (dbEmailConfirmation.Expiration <= DateTime.UtcNow)
                {
                    dbContext.EmailConfirmations.Remove(dbEmailConfirmation);
                    return new Result<EmailConfirmation>(StatusCode.ExpiredToken, null);
                }

                dbEmailConfirmation.User.IsConfirmed = true;
                dbContext.EmailConfirmations.Remove(dbEmailConfirmation);

                await dbContext.SaveChangesAsync();

                return new Result<EmailConfirmation>(StatusCode.OK, null);
            }
            else
            {
                return new Result<EmailConfirmation>(StatusCode.InvalidToken, null);
            }
        }
    }
}
