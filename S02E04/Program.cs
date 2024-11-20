using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Audio;
using OpenAI.Chat;
using Task9;

List<string> people = [];
List<string> hardware = [];

foreach (var filePath in Directory.EnumerateFiles("pliki_z_fabryki"))
{
    var fileExtension = Path.GetExtension(filePath);
    if (fileExtension == ".txt")
    {
        var text = await File.ReadAllTextAsync(filePath);
        await ExtractAsync(text, filePath);
    }
    else if (fileExtension == ".mp3")
    {
        await using var stream = File.OpenRead(filePath);
        var audioClient = new AudioClient("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        var result = await audioClient.TranscribeAudioAsync(stream, filePath);
        await ExtractAsync(result.Value.Text, filePath);
    }
    else if (fileExtension == ".png")
    {
        await using var stream = File.OpenRead(filePath);
        var binaryData = await BinaryData.FromStreamAsync(stream);
        var chatMessageContentPart = ChatMessageContentPart.CreateImagePart(binaryData, "image/png");
        var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        var result = await chatClient.CompleteChatAsync(
            ChatMessage.CreateUserMessage("Return text from the image"),
            ChatMessage.CreateUserMessage(chatMessageContentPart)
        );
        await ExtractAsync(result.Value.Content[0].Text, filePath);
    }
}

// foreach (var (filePath, text) in Data.Parsed)
// {
//     await ExtractAsync(text, filePath);
// }

people = people.OrderBy(x => x).ToList();
hardware = hardware.OrderBy(x => x).ToList();
Console.WriteLine($"People: {string.Join(", ", people)}");
Console.WriteLine($"Hardware: {string.Join(", ", hardware)}");

using var httpClient = new HttpClient();

var apiResponse = await httpClient.PostAsJsonAsync(
    "https://centrala.ag3nts.org/report",
    new
    {
        task = "kategorie",
        apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
        answer = new { people, hardware },
    });

Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");

return;

async Task ExtractAsync(string text, string filePath)
{
    const string systemPrompt = """
                                Check if the given file contains information about people or machines.
                                If you found information about people, verify whether it pertains to captured individuals or traces of their presence.
                                If you found information about machines, ensure it pertains to repaired hardware issues (ignore software-related issues).
                                Return as JSON in the format:
                                {
                                  "people: true | false,
                                  "hardware": true | false
                                }
                                """;

    var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    var chatResult = await chatClient.CompleteChatAsync(
        [
            ChatMessage.CreateSystemMessage(systemPrompt), 
            ChatMessage.CreateUserMessage(text),
        ],
        new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() }
    );
    Console.WriteLine($"""
                       File: {filePath},
                       Chat result: {chatResult.Value.Content[0].Text}
                       Text: {text}
                       """);
    Console.WriteLine();
    var chatResponse = JsonSerializer.Deserialize<ChatResponse>(chatResult.Value.Content[0].Text, JsonSerializerOptions.Web)!;
    var fileName = Path.GetFileName(filePath);
    if (chatResponse.People)
    {
        people.Add(fileName);
    }
    if (chatResponse.Hardware)
    {
        hardware.Add(fileName);
    }
}