using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

namespace S04E01;

public static class Helper
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public static async Task<string> AskAiAsync(this ChatClient chatClient, string question)
    {
        var response = await chatClient.CompleteChatAsync(question);
        return response.Value.Content[0].Text;
    }

    public static async Task<string> AskAiAsync(this ChatClient chatClient, Uri imageUri, string question)
    {
        var response = await chatClient.CompleteChatAsync(
            ChatMessage.CreateUserMessage(ChatMessageContentPart.CreateImagePart(imageUri),
                ChatMessageContentPart.CreateTextPart(question))
        );

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

    public static async Task<ReportEndpointResponse> TalkWithReportEndpointAsync(string message)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "photos",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = message,
        };
        var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<ReportEndpointResponse>();

        Console.WriteLine($"Report API response: {response}");

        return response!;
    }
}

public sealed record ReportEndpointResponse(int Code, string Message);