using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

namespace S03E05;

public static class Helper
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

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

    public static async Task<T[]> QueryDatabaseAsync<T>(string query)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "database",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            query,
        };
        var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/apidb", requestBody);
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<QueryDatabaseResponse<T>>(_jsonSerializerOptions);

        return response!.Reply;
    }

   public static async Task PostAnswersAsync(string path)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "connections",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = path,
        };
        var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

        Console.WriteLine($"API response: {responseJson}");
    }
}

public sealed record QueryDatabaseResponse<T>(T[] Reply);