namespace backend.DTOs
{
    public class UserRegistrationDTO
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string PasswordConfirmation { get; init; }
    }
}
