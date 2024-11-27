using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

namespace S04E03;

public static class Helper
{
    public static async Task<string> AskAiAsync(this ChatClient chatClient, string question)
    {
        var response = await chatClient.CompleteChatAsync(question);
        return response.Value.Content[0].Text;
    }

    public static async Task<T> AskAiAsync<T>(this ChatClient chatClient, string systemMessage, string userMessage)
    {
        var response = await chatClient.CompleteChatAsync(
            [
                ChatMessage.CreateSystemMessage(systemMessage),
                ChatMessage.CreateUserMessage(userMessage)
            ],
            new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() });

        var json = response.Value.Content[0].Text;

        var result = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Web)!;

        return result;
    }

    public static async Task<Dictionary<string, string>> GetQuestionsAsync()
    {
        using var httpClient = new HttpClient();
        using var httpResponseMessage = await httpClient.GetAsync($"https://centrala.ag3nts.org/data/{Environment.GetEnvironmentVariable("AIDEVS_KEY")}/softo.json");
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        return response!;
    }

    public static async Task PostAnswersAsync(Dictionary<string, string> answers)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "softo",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = answers,
        };
        using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

        Console.WriteLine($"API response: {responseJson}");
    }
}
