using CSharpFunctionalExtensions;
using InnTelegramBot.Domain.Models;

namespace InnTelegramBot.Application.Interfaces;

public interface IFnsService
{
    Task<Result<Company>> GetCompanyByInn(string inn);
}