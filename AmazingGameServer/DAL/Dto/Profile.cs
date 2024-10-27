namespace AmazingGameServer.DAL.Dto
{
    [GenerateSerializer, Alias(nameof(Profile))]
    public class Profile
    {
        [Id(0)]
        public int Id { get; set; }
        [Id(1)]
        public string Nickname { get; set; }
        [Id(2)]
        public int Coins { get; set; }

        [Id(3)]
        public List<Item>? Items { get; set; } = new();
    }
}
