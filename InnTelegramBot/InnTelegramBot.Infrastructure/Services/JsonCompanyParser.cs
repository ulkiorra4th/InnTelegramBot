using CSharpFunctionalExtensions;
using InnTelegramBot.Domain.Models;
using InnTelegramBot.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;

namespace InnTelegramBot.Infrastructure.Services;

internal sealed class JsonCompanyParser : IJsonCompanyParser
{
    public Result<Company> ParseFrom(JObject? json)
    {
        if (json is null) return Result.Failure<Company>("json is null");
        
        var items = json.Value<JArray>("items");
        if (items is null || items.Count == 0) return Result.Failure<Company>("Invalid json");
        
        var companyInfo = items.First!.Value<JObject>("ЮЛ") ?? items.First.Value<JObject>("ИП");
        if (companyInfo is null) return Result.Failure<Company>("Invalid json");
        
        var name = companyInfo.Value<string>("НаимПолнЮЛ") ?? companyInfo.Value<string>("ФИОПолн");
        if (name is null) return Result.Failure<Company>("invalid json");

        var addressSection = companyInfo.Value<JObject>("Адрес");
        if (addressSection is null) return Result.Failure<Company>("invalid json");

        var fullAddress = addressSection.Value<string>("АдресПолн");
        if (fullAddress is null) return Result.Failure<Company>("invalid json");
        
        var region = addressSection.Value<int?>("КодРегион");
        if (region is null) return Result.Failure<Company>("invalid json");
        
        var index = addressSection.Value<string>("Индекс");
        if (index is null) return Result.Failure<Company>("invalid json");

        return Result.Success(new Company(name, new Address(region.Value, index, fullAddress)));
    }
}