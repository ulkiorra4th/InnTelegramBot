using InnTelegramBot.Options;
using InnTelegramBot.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace InnTelegramBot.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
    {
        services
            .AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var botOptions = sp.GetRequiredService<IOptions<TelegramOptions>>().Value;
                return new TelegramBotClient(botOptions.ApiToken, httpClient);
            });
        
        services.AddSingleton<IUpdateHandler, UpdateHandler>();
        
        return services;
    }
}