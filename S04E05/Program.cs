using System.Text.Json;
using System.Text.RegularExpressions;
using OpenAI.Chat;
using S04E05;
using static S04E05.Helper;

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var questions = new Dictionary<string, string>
{
    ["01"] = "Do którego roku przeniósł się Rafał?",
    ["02"] = "Kto wpadł na pomysł, aby Rafał przeniósł się w czasie?",
    ["03"] = "Gdzie znalazł schronienie Rafał? Nazwij krótko to miejsce",
    ["04"] = "Którego dnia Rafał ma spotkanie z Andrzejem? (format: YYYY-MM-DD)",
    ["05"] = "Gdzie się chce dostać Rafał po spotkaniu z Andrzejem?",
};

var pageDescriptions = await LoadPageDescriptionsDatabaseAsync();
// foreach (var filePath in Directory.EnumerateFiles("notatki_rafala"))
// {
//     var fileName = Path.GetFileName(filePath);
//     var match = PageNumberFromFileNameRegex().Match(fileName);
//     var pageNumber = int.Parse(match.Value);
//     if (pageDescriptions.TryGetValue(pageNumber, out var pageDescription))
//     {
//         Console.WriteLine($"Page {pageNumber}: {pageDescription}");
//     }
//     else
//     {
//         Console.WriteLine($"Page {pageNumber}: No description available, asking AI for description...");
//         pageDescription = await chatClient.DescribeImageAsync(filePath);
//         pageDescriptions[pageNumber] = pageDescription;
//         Console.WriteLine($"Page {pageNumber}: {pageDescription}");
//     }
// }
//
// await SavePageDescriptionsDatabaseAsync(pageDescriptions);

var answers = questions.ToDictionary(x => x.Key, _ => null as string);
answers["01"] = "2019";
answers["02"] = "Adam";
answers["03"] = "jaskinia";
answers["04"] = "2024-11-12";

foreach (var (key, _) in answers.Where(x => string.IsNullOrWhiteSpace(x.Value)))
{
    var chatResponse = await chatClient.AskAiAsync<ChatResponse>(
        SystemPrompt(),
        questions[key]
    );

    answers[key] = chatResponse.Answer;

    Console.WriteLine($"Odpowiedź {key}: {chatResponse.Answer}");
}

await PostAnswersAsync(answers);

return;

static async Task<SortedDictionary<int, string>> LoadPageDescriptionsDatabaseAsync()
{
    const string fileName = "page_descriptions.json";

    if (!File.Exists(fileName))
    {
        return [];
    }

    var json = await File.ReadAllTextAsync(fileName);
    var pageDescriptions = JsonSerializer.Deserialize<Dictionary<int, string>>(json, JsonSerializerOptions.Web)!;

    return new SortedDictionary<int, string>(pageDescriptions, new PageNumberComparer());
}

static async Task SavePageDescriptionsDatabaseAsync(SortedDictionary<int, string> pageDescriptions)
{
    var json = JsonSerializer.Serialize(
        pageDescriptions,
        new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }
    );

    await File.WriteAllTextAsync("page_descriptions.json", json);
}

string SystemPrompt() =>
    $$"""
      # IDENTITY
      Jesteś ekspertem od analizowania i odpowiadania na pytanie na podstawie tekstu.

      # GOAL
      Twoim zadaniem jest dokładne przeanalizowanie tekstu i napisania odpowiedzi na pytanie.
      Odpowiedź wymagają przeanalizowania tekstu i kontekstu, a nie tylko wyszukania odpowiedzi.
      Odpowiadaj krótko i zwięźle.

      # OUTPUT
      Zwracasz wynik w postaci JSON:
      {
        "thinking": "<weż czas na przemyślenie, opisz swoje rozumowanie, a następnie znajdź odpowiedź na pytanie>",
        "answer": "<odpowiedź na pytanie>"
      }

      # EXAMPLES
      {
          "thinking": "Zacznę od przeczytania tekstu, a następnie odpowiem na pytania.",
          "answers": "2014-10-12"
      }

      {
          "thinking": "Przeczytam tekst, a następnie odpowiem na pytania.",
          "answers": "Andrzej"
      }

      <tekst>
      {{string.Join(Environment.NewLine, pageDescriptions.Select(x => $"Strona {x.Key}: {x.Value}"))}}
      </tekst>
      """;

partial class Program
{
    [GeneratedRegex(@"\d+(?=\.png$)")]
    private static partial Regex PageNumberFromFileNameRegex();
}