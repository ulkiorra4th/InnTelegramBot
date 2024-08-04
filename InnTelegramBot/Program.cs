using InnTelegramBot.Application.Extensions;
using InnTelegramBot.Extensions;
using InnTelegramBot.Infrastructure.Extensions;
using InnTelegramBot.Options;
using InnTelegramBot.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TelegramBotBackgroundService>();

// services to work with telegram
builder.Services.Configure<TelegramOptions>(builder.Configuration.GetTelegramSection());
builder.Services.AddTelegramBotClient();

// services to work with FNS api
builder.Services.AddFnsClient(builder.Configuration);
builder.Services.AddFnsService();

builder.Services.AddJsonCompanyParser();
builder.Services.AddMemoryCache();

var host = builder.Build();
host.Run();