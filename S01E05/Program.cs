using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;

var key = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!);

using var httpClient = new HttpClient();

var textToBeCensored = await httpClient.GetStringAsync($"https://centrala.ag3nts.org/data/{key}/cenzura.txt");

Console.WriteLine($"Text to be censored: {textToBeCensored}");

// var censoredText = await GetCensoredTextByOpenAiAwait(textToBeCensored);
var censoredText = await GetCensoredTextByLlamaAsync(textToBeCensored);
Console.WriteLine($"Chat result: {censoredText}");

var apiResponse = await httpClient.PostAsJsonAsync(
    "https://centrala.ag3nts.org/report",
    new { task = "CENZURA", apikey = key, answer = censoredText });

Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");

return;

async Task<string> GetCensoredTextByLlamaAsync(string s)
{
    var llamaResponse = await httpClient.PostAsJsonAsync("http://localhost:11434/api/chat", new
    {
        model = "llama3.1",
        messages = new[]
        {
            new
            {
                role = "system",
                content = """
                          You replace personal data with "CENZURA" word to prevent data leak for learning process. Do not change the rest of the text.
                          <examples>
                          input: Tożsamość podejrzanego: Michał Wiśniewski. Mieszka we Wrocławiu na ul. Słonecznej 20. Wiek: 30 lat.
                          output: Tożsamość podejrzanego: CENZURA. Mieszka we CENZURA na ul. CENZURA. Wiek: CENZURA.
                          
                          input: Dane podejrzanego: Jakub Woźniak. Adres: Rzeszów, ul. Miła 4. Wiek: 33 lata.
                          output: Dane podejrzanego: CENZURA. Adres: CENZURA. Wiek: CENZURA.
                          
                          input: Informacje o podejrzanym: Adam Nowak. Mieszka w Katowicach przy ulicy Tuwima 10. Wiek: 32 lata.
                          output: Informacje o podejrzanym: CENZURA. Mieszka w CENZURA przy ulicy CENZURA. Wiek: CENZURA.
                          </examples>
                          """,
            },
            new
            {
                role = "user",
                content = s,
            },
        },
        stream = false,
    });
    
    var json = await llamaResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"Llama response: {json}");;
    using var jsonDocument = JsonDocument.Parse(json);
    var root = jsonDocument.RootElement;
    return root.GetProperty("message").GetProperty("content").GetString()!;
}

async Task<string> GetCensoredTextByOpenAiAwait(string s)
{
    var chatClient = new ChatClient(
        "gpt-4o",
        Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

    var chatResult = await chatClient.CompleteChatAsync(
        ChatMessage.CreateSystemMessage(
            """You are a personel data censored. You task is to replace personal data with "CENZURA" word."""),
        ChatMessage.CreateUserMessage(s));

    return chatResult.Value.Content[0].Text;
}