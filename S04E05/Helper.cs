using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

namespace S04E05;

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

    public static async Task<string> DescribeImageAsync(this ChatClient chatClient, string imageFilePath)
    {
        await using var stream = File.OpenRead(imageFilePath);
        var binaryData = await BinaryData.FromStreamAsync(stream);
        var chatMessageContentPart = ChatMessageContentPart.CreateImagePart(binaryData, "image/png");

        var response = await chatClient.CompleteChatAsync(
            ChatMessage.CreateSystemMessage("""
                                            # IDENTITY
                                            Jesteś ekspertem od opisania obrazów.
                                            
                                            # GOAL
                                            Twoim zadaniem jest dokładne opisanie tego co jest na obrazie w języku polskim.
                                            
                                            # RULES
                                            Jeśli na obrazie znajduje się tekst, to zrób jest transkrypcję oraz opisz inne szczegóły.
                                            Jeśli na obrazie znajduje się coś innego, to opisz to w sposób jak najbardziej precyzyjny.
                                            """),
            ChatMessage.CreateUserMessage(chatMessageContentPart)
        );

        return response.Value.Content[0].Text;
    }

    public static async Task<Dictionary<string, string>> GetQuestionsAsync()
    {
        using var httpClient = new HttpClient();
        using var httpResponseMessage = await httpClient.GetAsync($"https://centrala.ag3nts.org/data/{Environment.GetEnvironmentVariable("AIDEVS_KEY")}/notes.json");
        var response = await httpResponseMessage.Content.ReadFromJsonAsync<Dictionary<string, string>>();

        return response!;
    }

    public static async Task PostAnswersAsync(Dictionary<string, string> answers)
    {
        using var httpClient = new HttpClient();
        var requestBody = new
        {
            task = "notes",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = answers,
        };
        using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

        Console.WriteLine($"API response: {responseJson}");
    }
}
