namespace InnTelegramBot.Data;

internal sealed class Command
{
    public const string StartCommandConst = "/start";
    public const string HelloCommandConst = "/hello";
    public const string HelpCommandConst = "/help";
    public const string InnCommandConst = "/inn";
    public const string LastCommandConst = "/last";
    public const string OkvedCommandConst = "/okved";
    public const string EgrulCommandConst = "/egrul";

    public string Syntax { get; }
    public string Description { get; }

    private Command(string syntax, string description)
    {
        Syntax = syntax;
        Description = description;
    }

    public static Command HelloCommand { get; } = 
        new Command(HelloCommandConst, "получить информацию об авторе бота");
    
    public static Command HelpCommand  { get; } =
        new Command(HelpCommandConst, "вывести информацию о доступных командах");
    
    public static Command InnCommand { get; } =
        new Command(InnCommandConst + " ИНН1, ИНН2, ...", "получить наименования и адреса компаний по ИНН");
    
    public static Command LastCommand  { get; } =
        new Command(LastCommandConst, "повторить последнее действие бота");

    public static IReadOnlyCollection<Command> AllCommands { get; } = new[]
    {
        HelpCommand, 
        HelloCommand, 
        InnCommand, 
        LastCommand
    };
};