namespace AmazingGameCLient.Models
{
    internal class UserProfile
    {
        public int Id { get; set; }
        public string? Nickname { get; set; }
        public Item[]? UserItems { get; set; }
        public int Coins { get; set; }
    }
}
