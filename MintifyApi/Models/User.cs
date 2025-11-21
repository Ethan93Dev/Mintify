namespace MintifyApi.Models
{
    public class User
    {
        public int Id { get; set; }  // Primary key

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;
    }
}