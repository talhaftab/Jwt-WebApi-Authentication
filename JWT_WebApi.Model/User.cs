namespace JWT.WebApi.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
    }
}