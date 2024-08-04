using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace InnTelegramBot.Services;

internal sealed class TelegramBotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;

    public TelegramBotBackgroundService(ITelegramBotClient botClient, IUpdateHandler updateHandler)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _botClient.ReceiveAsync(
                updateHandler: _updateHandler,
                receiverOptions: receiverOptions, 
                cancellationToken: stoppingToken);
        }
    }
}