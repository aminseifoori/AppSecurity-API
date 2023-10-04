namespace AppSecurity_API.Settings
{
    public class JwtSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string JwtSecters { get; set; }
        public int Expires { get; set; }
    }
}
