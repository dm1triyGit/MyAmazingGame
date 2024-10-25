namespace AmazingGameServer.DAL.Dto
{
    [GenerateSerializer, Alias(nameof(Item))]
    public class Item
    {
        [Id(0)]
        public int Id { get; set; }
        [Id(1)]
        public string Name { get; set; }
        [Id(2)]
        public int Price { get; set; }

        public List<Profile> Profile { get; set; } = new();
    }
}
