namespace Blog;

public static class Configuration
{
    public static string JwtKey = "D5D06841C6666AB42ADB57BADFAF3AA4A19E8D40B478C945FFD8C65DC0A31E80";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_balta_chave_da_api_super_secreta";

    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
