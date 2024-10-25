namespace AmazingGameServer.DAL.Dto
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }


        public List<Profile> Profile { get; set; } = new();
    }
}
