using Microsoft.Extensions.Configuration;

namespace InnTelegramBot.Infrastructure.Extensions;

internal static class ConfigurationsExtensions
{
    public static IConfigurationSection GetFnsClientSection(this IConfiguration configuration) =>
        configuration.GetSection("FnsClient");
    
}