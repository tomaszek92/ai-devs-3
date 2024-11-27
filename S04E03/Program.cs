using OpenAI.Chat;
using S04E03;
using static S04E03.Helper;

var questions = await GetQuestionsAsync();

Console.WriteLine("Questions:");
foreach (var (key, value) in questions)
{
    Console.WriteLine($"{key}: {value}");
}

using var httpClient = new HttpClient();
var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var answers = new Dictionary<string, string>();
List<Page> pages = [];
var url = "https://softo.ag3nts.org";
const int MaxIterations = 20;
for (var i = 1; i <= MaxIterations; i++)
{
    Console.WriteLine($"Analyzing page {url} | ({i}/{MaxIterations})");
    var pageHtml = await httpClient.GetStringAsync(url);
    var prompt = Prompt();
    var pageAnalysis = await chatClient.AskAiAsync<PageAnalysis>(prompt, pageHtml);
    pages.Add(new Page(url, pageAnalysis.Summary));
    Console.WriteLine($"Summary: {pageAnalysis.Summary}");
    foreach (var (questionId, answer) in pageAnalysis.Answers)
    {
        Console.WriteLine($"Got answer for question {questionId}: {answer}");
        answers[questionId] = answer;
        questions.Remove(questionId);
    }

    if (!string.IsNullOrWhiteSpace(pageAnalysis.NextPage))
    {
        if (Uri.TryCreate(pageAnalysis.NextPage, UriKind.RelativeOrAbsolute, out var uri))
        {
            url = uri.IsAbsoluteUri ? uri.AbsoluteUri : new Uri(new Uri(url), uri).AbsoluteUri;
        }
        else
        {
            Console.WriteLine($"Invalid URL: {pageAnalysis.NextPage}");
            break;
        }
    }
    else
    {
        break;
    }
}

if (questions.Count == 0)
{
    await PostAnswersAsync(answers);
}
else
{
    Console.WriteLine("Not all questions were answered.");
    Console.WriteLine("Answers:");
    foreach (var (key, value) in answers)
    {
        Console.WriteLine($"{key}: {value}");
    }
    Console.WriteLine("Remaining questions:");
    foreach (var (key, value) in questions)
    {
        Console.WriteLine($"{key}: {value}");
    }
}

return;

string Prompt() =>
  $$"""
    # IDENTITY
    Jesteś ekspertem w analizowaniu zawartości stron internetowych.

    # GOAL
    Twoim zadaniem jest znalezienie odpowiedzi na następujące pytania:
    {{string.Join(Environment.NewLine, questions.Select(x => $"- {x.Key}: {x.Value}"))}}

    # STEPS
    1. Przeczytaj treść strony internetowej.
    2. Spróbuj znaleźć odpowiedź na pytanie.
    3. Stwórz podsumowanie zawartości strony.
    4. Podejmij decyzję czy przechodzisz do następnej strony.
    4.1. Masz już wiedzę na temat odwiedzonych stron, więc nie możesz wrócić do poprzednich:
    {{(pages.Count > 0 ? string.Join(Environment.NewLine, pages.Select(x => $"- {x.Url}: {x.Summary}")) : "Brak odwiedzonych stron.")}}

    # OUTPUT
    Zwracasz wynik w postaci JSON:
    {
      "summary": <podsumowanie zawartości strony>,
      "nextPage": <adres URL kolejnej strony>
      "answers": <odpowiedzi na pytania>
    }

    # EXAMPLE
    1. 
    {
      "summary": "Strona zawiera informacje o firmie Softo.",
      "nextPage": "https://softo.ag3nts.org/kontakt",
      "answers": {} // pusty obiekt, gdy nie znaleziono odpowiedzi

    2.
    {
      "summary": "Strona zawiera informacje na temat portfolio firmy Softo.",
      "nextPage": null, // brak kolejnej strony, bo masz odpowiedzi na wszystkie pytania
      "answers": {
        "question1Id": "odpowiedź na pytanie 1",
        "question2Id": "odpowiedź na pytanie 2",
        "question3Id": "odpowiedź na pytanie 3"
      }
    }
    """;