namespace AmazingGameServer.DAL.Dto
{
    public class Profile
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int Coins { get; set; }

        public List<Item> Items { get; set; } = new();
    }
}
