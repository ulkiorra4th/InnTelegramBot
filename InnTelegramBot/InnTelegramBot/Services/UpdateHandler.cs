using System.Text;
using InnTelegramBot.Application.Interfaces;
using InnTelegramBot.Data;
using InnTelegramBot.Models;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace InnTelegramBot.Services;

internal sealed class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<TelegramBotBackgroundService> _logger;
    private readonly IFnsService _fnsService;
    private readonly IMemoryCache _cache;

    public UpdateHandler(ILogger<TelegramBotBackgroundService> logger, IFnsService fnsService, IMemoryCache cache)
    {
        _logger = logger;
        _fnsService = fnsService;
        _cache = cache;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(botClient, message),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError("HandleError: {Exception}", exception);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
    
    private async Task OnMessage(ITelegramBotClient botClient, Message msg)
    {
        _logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        var sentMessage = await (messageText.Split(' ')[0] switch
        {
            Command.HelloCommandConst => SendHelloMessage(botClient, msg),
            Command.HelpCommandConst => SendUsageMessage(botClient, msg),
            Command.InnCommandConst => SendCompanyInfoByInn(botClient, msg),
            Command.LastCommandConst => RepeatLastCommand(botClient, msg),
            _ => SendUnknownCommandMessage(botClient, msg)
        });
        
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }

    private async Task<Message> RepeatLastCommand(ITelegramBotClient botClient, Message msg)
    {
        if (!_cache.TryGetValue(msg.Chat.Id, out ExecutedCommand? lastExecutedCommand))
            return await botClient.SendTextMessageAsync(msg.Chat, "Вы еще не совершали никаких действий.", 
                parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());

        return await lastExecutedCommand!.Delegate(lastExecutedCommand.BotClient, lastExecutedCommand.Message);
    }
    
    private async Task<Message> SendUsageMessage(ITelegramBotClient botClient, Message msg)
    {
        _cache.Set(msg.Chat.Id, new ExecutedCommand(SendUsageMessage, botClient, msg));
        
        var usageMessage = new StringBuilder("<b><u>Команды</u></b>:");

        foreach (var command in Command.AllCommands)
        {
            usageMessage.Append($"\n{command.Syntax} - {command.Description}");
        }

        return await botClient.SendTextMessageAsync(msg.Chat, usageMessage.ToString(), parseMode: ParseMode.Html, 
            replyMarkup: new ReplyKeyboardRemove());
    }
   
    private async Task<Message> SendHelloMessage(ITelegramBotClient botClient, Message msg)
    {
        _cache.Set(msg.Chat.Id, new ExecutedCommand(SendHelloMessage, botClient, msg));
        
        var helloMessage = 
            "*name:* Мухтаров Рамин\n*email:* ramin.muhtarov@gmail.com\n*github:* https://github.com/ulkiorra4th";

        return await botClient.SendTextMessageAsync(msg.Chat, helloMessage, parseMode: ParseMode.Markdown, 
            replyMarkup: new ReplyKeyboardRemove());
    }
    
    private async Task<Message> SendUnknownCommandMessage(ITelegramBotClient botClient, Message msg)
    {
        var message = $"Неизвестная команда. Список команд: {Command.HelpCommandConst}";
        return await botClient.SendTextMessageAsync(msg.Chat, message, parseMode: ParseMode.Markdown, 
            replyMarkup: new ReplyKeyboardRemove());
    }
    
    private async Task<Message> SendCompanyInfoByInn(ITelegramBotClient botClient, Message msg)
    {
        _cache.Set(msg.Chat.Id, new ExecutedCommand(SendCompanyInfoByInn, botClient, msg));
        
        var splittedMessage = msg.Text!.Split(' ');

        if (splittedMessage.Length < 2)
            return await botClient.SendTextMessageAsync(msg.Chat, 
                $"Использование: {Command.InnCommandConst} ИНН1, ИНН2, ... ", 
                parseMode: ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());
        
        var inns = splittedMessage.ToList().GetRange(1, splittedMessage.Length - 1);
        var message = new StringBuilder();
        
        foreach (var inn in inns)
        {
            var companyResult = await _fnsService.GetCompanyByInn(inn);
            if (companyResult.IsFailure)
            {
                _logger.LogError(companyResult.Error);
                return await botClient.SendTextMessageAsync(msg.Chat, 
                    "Что-то пошло не так. Проверьте корректность введенных данных и повторите попытку.",
                    parseMode: ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());
            }

            var company = companyResult.Value;
        
            message.Append($"*Название:* {company.Name}\n*Номер региона:* {company.Address.Region}\n"+
                           $"*Индекс:* {company.Address.Index}\n*Полный адрес:* {company.Address.FullAddress}\n\n");
        }

        return await botClient.SendTextMessageAsync(msg.Chat, message.ToString(), parseMode: ParseMode.Markdown, 
            replyMarkup: new ReplyKeyboardRemove());
    }
    
    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}