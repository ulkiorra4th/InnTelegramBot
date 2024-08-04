using CSharpFunctionalExtensions;
using InnTelegramBot.Application.Interfaces;
using InnTelegramBot.Application.Interfaces.Infrastructure;
using InnTelegramBot.Domain.Models;

namespace InnTelegramBot.Application.Services;

internal sealed class FnsService : IFnsService
{
    private readonly IFnsClient _fnsClient;

    public FnsService(IFnsClient fnsClient)
    {
        _fnsClient = fnsClient;
    }

    public async Task<Result<Company>> GetCompanyByInn(string inn) => await _fnsClient.GetCompanyByInn(inn);
}