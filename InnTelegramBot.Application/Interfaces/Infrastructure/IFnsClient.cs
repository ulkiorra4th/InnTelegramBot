using CSharpFunctionalExtensions;
using InnTelegramBot.Domain.Models;

namespace InnTelegramBot.Application.Interfaces.Infrastructure;

public interface IFnsClient
{
    Task<Result<Company>> GetCompanyByInn(string inn);
}