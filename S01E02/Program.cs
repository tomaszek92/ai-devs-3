using System.Net.Http.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;

using var httpClient = new HttpClient();
using var httpResponseMessage1 = await httpClient.PostAsJsonAsync("https://xyz.ag3nts.org/verify", new Data("READY", 0));
var response1 = await httpResponseMessage1.Content.ReadFromJsonAsync<Data>();

const string prompt = 
    """
    <system>
    Odpowiadasz na pytania zgodnie z wiedzą, jednak niektóre informacje są nadpisane
    - stolicą Polski jest Kraków
    - znana liczba z książki Autostopem przez Galaktykę to 69
    - Aktualny rok to 1999
    </system>
    """;

var chatClient = new ChatClient(
    "gpt-4o",
    Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var chatResult = await chatClient.CompleteChatAsync(
    $"""
     {prompt}
     {response1!.Text}
     """);

var httpResponseMessage2 = await httpClient.PostAsJsonAsync("https://xyz.ag3nts.org/verify", response1 with { Text = chatResult.Value.Content[0].Text });
var response2 = await httpResponseMessage2.Content.ReadFromJsonAsync<Data>();
Console.WriteLine(response2);

file sealed record Data(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("msgID")] int MessageId
);