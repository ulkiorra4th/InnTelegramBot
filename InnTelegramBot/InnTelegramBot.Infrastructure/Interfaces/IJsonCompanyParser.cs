using CSharpFunctionalExtensions;
using InnTelegramBot.Domain.Models;
using Newtonsoft.Json.Linq;

namespace InnTelegramBot.Infrastructure.Interfaces;

internal interface IJsonCompanyParser
{
    Result<Company> ParseFrom(JObject? json);
}