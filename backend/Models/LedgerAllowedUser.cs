namespace backend.Models
{
    public class LedgerAllowedUser
    {
        public required Guid UserId { get; set; }
        public required Guid LedgerId { get; set; }
        public required Guid AccessLevelId { get; set; }

        public DateTime? Expiration { get; set; }

        //navigation properties
        public User User { get; set; } = null!;
        public Ledger Ledger { get; set; } = null!;
        public LedgerAccessLevel AccessLevel { get; set; } = null!;
    }
}
