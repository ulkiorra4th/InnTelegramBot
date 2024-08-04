using CSharpFunctionalExtensions;
using InnTelegramBot.Application.Interfaces.Infrastructure;
using InnTelegramBot.Domain.Models;
using InnTelegramBot.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;

namespace InnTelegramBot.Infrastructure.Services;

internal sealed class FnsClient : IFnsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    private readonly IJsonCompanyParser _jsonCompanyParser;

    public FnsClient(HttpClient httpClient, string apiKey, IJsonCompanyParser jsonCompanyParser)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _jsonCompanyParser = jsonCompanyParser;
    }

    public async Task<Result<Company>> GetCompanyByInn(string inn)
    {
        var requestUrl = $"egr?key={_apiKey}&req={inn}";
        HttpResponseMessage response;
        
        try
        {
            response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
                return Result.Failure<Company>($"Error. Status code: {response.StatusCode}");
        }
        catch (HttpRequestException e)
        {
            return Result.Failure<Company>(e.Message);
        }
        
        var json = JObject.Parse(await response.Content.ReadAsStringAsync());
        var companyParseResult = _jsonCompanyParser.ParseFrom(json);

        return companyParseResult.IsFailure
            ? companyParseResult
            : Result.Success(companyParseResult.Value);
    }
}