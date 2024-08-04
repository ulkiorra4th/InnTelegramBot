using InnTelegramBot.Application.Interfaces;
using InnTelegramBot.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InnTelegramBot.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFnsService(this IServiceCollection services) =>
        services.AddSingleton<IFnsService, FnsService>();
    
}