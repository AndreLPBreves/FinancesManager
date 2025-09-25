namespace backend.Models
{
    public class EmailConfirmation
    {
        public required Guid UserId { get; init; }
        public Guid Key { get; init; } = Guid.CreateVersion7();
        public DateTime Expiration { get; init; } = DateTime.UtcNow.AddDays(1);
        public bool Used { get; set; } = false;

        //Navigation properties
        public User User { get; set; } = null!;
    }
}
