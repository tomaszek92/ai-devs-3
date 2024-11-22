using System.Text.Json;
using OpenAI.Chat;
using S03E04;
using static S03E04.Helper;

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var note = await File.ReadAllTextAsync("barbara.txt");

var generalInfo = await chatClient.AskAiAsync<Response>($$"""
                                                             Zwróć listę osób (tylko imię) i miast, które są wymienione w notatce.
                                                             Słowa w wyniku mają być w mianowniku, bez polskich znaków i wielkimi literami. 
                                                              Zwróć to jako json:
                                                             ```json
                                                             {
                                                                 "places": ["<miasto1>", "<miasto2>", "<miasto3>"],
                                                                 ""people: ["<osoba1>", "<osoba2>", "<osoba3>"],
                                                             }
                                                             ```
                                                             <notatka>
                                                             {{note}}
                                                             </notatka>
                                                             """);

Console.WriteLine($"Początkowe informacje: {JsonSerializer.Serialize(generalInfo)}");

const int MaxApiQuestions = 10;
for (var i = 1; i <= MaxApiQuestions; i++)
{
    Console.WriteLine($"------------------ Iteracja {i} ------------------");
    var askPersonTasks = generalInfo.People.Select(AskAboutPersonAsync);
    List<(string person, string places)> personsInfo = [];
    await foreach (var personInfo in Task.WhenEach(askPersonTasks))
    {
        personsInfo.Add(await personInfo);
    }

    var askPlaceTasks = generalInfo.Places.Select(AskAboutPlaceAsync);
    List<(string place, string persons)> placesInfo = [];
    await foreach (var placeInfo in Task.WhenEach(askPlaceTasks))
    {
        placesInfo.Add(await placeInfo);
    }

    var prompt = Prompt(personsInfo.ToArray(), placesInfo.ToArray());

    Console.WriteLine($"PROMPT: {prompt}");

    generalInfo = await chatClient.AskAiAsync<Response>(prompt);

    Console.WriteLine($"ODPOWIEDŹ: {JsonSerializer.Serialize(generalInfo)}");

    if (!string.IsNullOrWhiteSpace(generalInfo.BarbaraPlace))
    {
        await PostAnswersAsync(generalInfo.BarbaraPlace);
        break;
    }
}

return;

string Prompt((string person, string places)[] personsInfo, (string place, string persons)[] placesInfo) =>
    $$"""
      Na podstawie dostarczonych informacji spróbuj odgadnąć, w którym mieście znajduje się BARBARA.
      BARBARA nie znajduje się w KRAKOW.

      <notatka na temat BARBARY>
      {{note}}
      </notatka na temat BARBARY>

      <dostarczone informacje>
      List miejsc, w których widziano:
      {{string.Join(Environment.NewLine, personsInfo.Select(x => $"{x.person}: {x.places}"))}}
      Miejsca odwiedzone przez konkretne osoby:
      {{string.Join(Environment.NewLine, placesInfo.Select(x => $"{x.place}: {x.persons}"))}}
      </dostarczone informacje>

      <response>
      Jeśli udało się odgadnąć, w którym mieście znajduje się BARBARA, zwróć:
      ```json
      {
          "barbaraPlace": "<miasto>"
      }
      ```
      Jeśli nie udało się odgadnąć i potrzebujesz dodatkowych informacji, zwróć:
      ```json
      {
          "places": ["<miasto1>", "<miasto2>", "<miasto3>"],
          ""people: ["<osoba1>", "<osoba2>", "<osoba3>"],
      }
      ```
      </response>
      """;

