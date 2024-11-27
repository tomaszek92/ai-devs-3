using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

namespace S03E04;

public static class Helper
{
    public static async Task<string> AskAiAsync(this ChatClient chatClient, string question)
    {
        var response = await chatClient.CompleteChatAsync(question);
        return response.Value.Content[0].Text;
    }

    public static async Task<T> AskAiAsync<T>(this ChatClient chatClient, string question)
    {
        var response = await chatClient.CompleteChatAsync(
            [question],
            new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() });

        var json = response.Value.Content[0].Text;

        var result = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Web)!;

        return result;
    }

    public static async Task<(string person, string places)> AskAboutPersonAsync(string person) => (person, await AskAboutAsync("people", person));

    public static async Task<(string place, string persons)> AskAboutPlaceAsync(string place) => (place, await AskAboutAsync("places", place));

    private static async Task<string> AskAboutAsync(string entity, string query)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            query,
        };
        using var httpResponseMessage = await httpClient.PostAsJsonAsync($"https://centrala.ag3nts.org/{entity}", requestBody);
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<CentralaResponse>();

        return response!.Message;
    }

    public static async Task PostAnswersAsync(string city)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "loop",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = city,
        };
        using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

        Console.WriteLine($"API response: {responseJson}");
    }
}

public sealed record CentralaResponse(int Code, string Message);