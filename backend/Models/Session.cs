using System.Diagnostics.CodeAnalysis;
using backend.Services;

namespace backend.Models
{
    public class Session
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();
        public required Guid UserId { get; init; }
        public User? User { get; set; }
        public string Jwt { get; set; } = string.Empty;
        public required DateTime Creation { get; init; } = DateTime.UtcNow;
        public required DateTime Expiration { get; set; }
        public DateOnly? Exclusion { get; set; } = null;

        [SetsRequiredMembers]
        private Session(Guid userId, DateTime expiration)
        {
            UserId = userId;
            Expiration = expiration;
        }

        public static Session Create(User user, double expirationMinutes, JwtService jwtService)
        {
            var newSession = new Session(
                userId: user.Id,
                expiration: DateTime.UtcNow.AddMinutes(expirationMinutes)
            );
            newSession.Jwt = jwtService.GenerateToken(user, newSession);

            return newSession;
        }
    }
}
