namespace ENDE.Models.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryData { get; set; }
    }
}
