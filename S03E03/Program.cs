using System.Net.Http.Json;
using System.Text.RegularExpressions;
using OpenAI.Chat;

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var tables = await CallDatabaseApiAsync("show tables");
var answer1 = await AskAiAsync($"""
                                   Na podstawie listy tabel zwróć ich same nazwy oddzielone przecinkami.
                                   {tables}
                                   """);
Console.WriteLine($"Lista tabel: {answer1}");

List<string> tableSchemas = [];
foreach (var tableName in answer1.Split(',').Select(x => x.Trim()))
{
    var tableSchema = await CallDatabaseApiAsync($"show create table {tableName}");
    tableSchemas.Add(tableSchema);
    Console.WriteLine($"Table schema: {tableSchema}");
}

var prompt = tableSchemas.Aggregate(
    "Na podstawie schematów tabel, napisz zapytanie, które zwróci numery ID czynnych datacenter, które zarządzane są przez menadżerów, którzy aktualnie przebywają na urlopie (są nieaktywni).",
    (current, tableSchema) => current + $"""
                                         <table_schema>
                                         {tableSchema}
                                         </table_schema>
                                         """);

var answer2 = await AskAiAsync(prompt);
Console.WriteLine($"Query: {answer2}");

var query = ExtractSqlBlock(answer2);
Console.WriteLine($"Query: {query}");
var result = await CallDatabaseApiAsync(query);

var dcIdsStr = await AskAiAsync($"""
                                 Wyodrębnij same ID z tekstu i zwróć przedzielone przecinkami.
                                 <tekst>
                                 {result}
                                 </tekst>
                                 """);

Console.WriteLine($"Datacenter IDs: {dcIdsStr}");

await PostAnswersAsync(dcIdsStr.Split(',').Select(x => x.Trim()).ToArray());

return;

static async Task<string> CallDatabaseApiAsync(string query)
{
    using var client = new HttpClient();
    using var httpResponseMessage = await client.PostAsJsonAsync("https://centrala.ag3nts.org/apidb", new
    {
        task = "database",
        apikey = Environment.GetEnvironmentVariable("AIDEVS_KEY"),
        query,
    });

    var json =  await httpResponseMessage.Content.ReadAsStringAsync();

    return json;
}

async Task<string> AskAiAsync(string question)
{
    var response = await chatClient.CompleteChatAsync(question);
    return response.Value.Content[0].Text;
}

static string ExtractSqlBlock(string input)
{
    const string pattern = @"```sql\s*(.*?)\s*```";
    var match = Regex.Match(input, pattern, RegexOptions.Singleline);

    return match.Success ? match.Groups[1].Value : string.Empty;
}

async Task PostAnswersAsync(string[] dcIds)
{
    using var httpClient = new HttpClient();

    var requestBody = new
    {
        task = "database",
        apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
        answer = dcIds,
    };

    var apiResponse = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);

    Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");
}