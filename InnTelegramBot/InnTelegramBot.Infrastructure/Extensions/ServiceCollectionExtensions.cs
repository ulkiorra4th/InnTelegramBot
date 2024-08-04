using InnTelegramBot.Application.Interfaces.Infrastructure;
using InnTelegramBot.Infrastructure.Interfaces;
using InnTelegramBot.Infrastructure.Options;
using InnTelegramBot.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InnTelegramBot.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFnsClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FnsClientOptions>(configuration.GetFnsClientSection());
        
        services
            .AddHttpClient("fns_client")
            .AddTypedClient<IFnsClient>((httpClient, serviceProvider) =>
            {
                httpClient.BaseAddress = new Uri(configuration.GetFnsClientSection().GetValue<string>("BaseAddress") ?? 
                                                 throw new Exception("invalid app settings. FnsClient.BaseAddress is incorrect"));
                var apiKey = serviceProvider.GetRequiredService<IOptions<FnsClientOptions>>().Value.ApiToken;
                return new FnsClient(httpClient, apiKey, 
                    serviceProvider.GetRequiredService<IJsonCompanyParser>());
            });

        return services;
    }

    public static IServiceCollection AddJsonCompanyParser(this IServiceCollection services)
    {
        return services.AddSingleton<IJsonCompanyParser, JsonCompanyParser>();
    }
}