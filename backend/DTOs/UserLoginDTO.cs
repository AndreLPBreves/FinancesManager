namespace backend.DTOs
{
    public class UserLoginDTO
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
