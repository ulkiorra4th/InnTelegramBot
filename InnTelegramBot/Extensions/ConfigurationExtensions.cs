namespace InnTelegramBot.Extensions;

internal static class ConfigurationExtensions
{
    public static IConfigurationSection GetTelegramSection(this IConfiguration configuration) => 
        configuration.GetSection("Telegram");
    
}