using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;

var json = await File.ReadAllTextAsync("data.json");
var data = JsonSerializer.Deserialize<Data>(json, JsonSerializerOptions.Web);

using var httpClient = new HttpClient();
var chatClient = new ChatClient(
    "gpt-4o",
    Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

for (var i = 0; i < data!.TestData.Length; i++)
{
    var testData = data.TestData[i];
    var sum = CalculateStringSum(testData.Question);
    if (sum != testData.Answer)
    {
        data.TestData[i] = testData with { Answer = sum };
    }

    if (testData.Test is not null)
    {
        var userChatMessage = ChatMessage.CreateUserMessage($"{testData.Test.Q} answer with one word");
        var chatResult = await chatClient.CompleteChatAsync(userChatMessage);
        Console.WriteLine($"{testData.Test.Q}: {chatResult.Value.Content[0].Text}");
        data.TestData[i] = testData with { Test = testData.Test with { A = chatResult.Value.Content[0].Text } };
    }
}

var response = new { task = "JSON", apikey = Environment.GetEnvironmentVariable("AIDEVS_KEY"), answer = data };
using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", response);
var flagResponse = await httpResponseMessage.Content.ReadAsStringAsync();
Console.WriteLine(flagResponse);

return;

static int CalculateStringSum(string input)
{
    var parts = input.Split('+');
    var num1 = int.Parse(parts[0].Trim());
    var num2 = int.Parse(parts[1].Trim());

    return num1 + num2;
}


file sealed record Data(
    [property: JsonPropertyName("apikey")] Guid ApiKey,
    string Description,
    string Copyright,
    [property: JsonPropertyName("test-data")] TestData[] TestData
);

file sealed record TestData(string Question, int Answer, Test? Test);

file sealed record Test(string Q, string A);