namespace AmazingGameServer.BLL.Options
{
    public class AuthOptions
    {
        public string Issuer { get; set; } 
        public string Audience { get; set; }
        public string Key { get; set; }
        public int LifetimeHours { get; set; }
    }
}
