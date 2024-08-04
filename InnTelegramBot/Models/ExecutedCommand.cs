using Telegram.Bot;
using Telegram.Bot.Types;

namespace InnTelegramBot.Models;

internal sealed class ExecutedCommand
{
    public delegate Task<Message> Command(ITelegramBotClient botClient, Message msg);
    
    public Command Delegate { get; init; }
    public ITelegramBotClient BotClient { get; init; }
    public Message Message { get; init; }

    public ExecutedCommand(Command @delegate, ITelegramBotClient botClient, Message message)
    {
        Delegate = @delegate;
        BotClient = botClient;
        Message = message;
    }
}