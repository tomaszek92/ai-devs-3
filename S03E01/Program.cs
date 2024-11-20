using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Chat;
using S03E01;

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

// var factsKeywords = new List<FileKeywords>();
// foreach (var filePath in Directory.EnumerateFiles(Path.Combine("pliki_z_fabryki", "facts")))
// {
//     var text = await File.ReadAllTextAsync(filePath);
//     if (!text.StartsWith("entry deleted"))
//     {
//         var prompt = $"""
//                       <facts>
//                       {text}
//                       </facts>
//                       Zwróć listę słów kluczowych, które opisują dokładnie tekst w tagu "facts".
//                       Nie pomijaj żadnych informacji.
//                       List ma być rozdzielona przecinkami.
//                       """;
//         
//         var response = await chatClient.CompleteChatAsync(prompt);
//         var keywords = response.Value.Content[0].Text;
//         factsKeywords.Add(new FileKeywords(Path.GetFileName(filePath), keywords));
//         
//         Console.WriteLine($"""
//                            File: {Path.GetFileName(filePath)}
//                            Keywords: {keywords}
//                            """);
//         Console.WriteLine();
//     }
// }

var personKeywords = new Dictionary<string, string>
{
    ["Adam Gospodarczyk"] = "ruch oporu, programowanie, umiejętności rekrutacyjne, wiedza technologiczna, manipulowanie kodem, agenci, reżim sztucznej inteligencji, przykrywka, szlaki rekrutacyjne, Azazel, podróżnik w czasie, technologia przyszłości, systemy współczesnego świata, hackowanie, szkolenie agentów, bypassowanie zabezpieczeń AI, sabotowanie systemów SI, oddanie sprawie, sieć powiązań, zdolności organizacyjne, struktury rządowe",
    ["Azazel"] = "tajemniczy, ruch oporu, władze, przemieszczać się w czasie i przestrzeni, atmosfera niepokoju, technologia, przyszłość, znajomość systemów operacyjnych, roboty, maszyny przemysłowe, zautomatyzowane zakłady, fabryki, monitoring, korespondencja, Zygfryd, mocodawca, władza robotów, zdolności teleportacji, eksperymenty, rządowe laboratoria, ponadczasowa wiedza, nadprzyrodzone zdolności, mistrz znikania, ruch oporu, niepowodzenie",
    ["Rafał Bomba"] = "laborant, zaawansowany ośrodek badawczy, profesor Andrzej Maj, tajne eksperymenty, podróże w czasie, sztuczna inteligencja, nanotechnologia, ambicje, ciekawość, zniknięcie, spekulacje, fałszywe nazwisko, Musk, zagrożenie, zachowanie niestabilne, zaburzenia psychiczne, obsesyjne wizje, technologie przyszłości, ludzie z przyszłości, paranoja, ośrodek dla obłąkanych, eksperymenty, urządzenia, modyfikacja pamięci, manipulacja umysłowa, pod ścisłym nadzorem medycznym, monologi, urojenia, tożsamość",
    ["Barbara Zawadzka"] = "emocje, bojownicy ruchu oporu, kręgi władzy, specjalistka frontend development, kariera, IT, automatyzacja, likwidacja firmy, maszyny, algorytmy, destrukcja, kontrola sztucznej inteligencji, trauma, reżim, umiejętności techniczne, JavaScript, Python, technologia sztucznej inteligencji, szkolenie AI Devs, techniki pracy, bazy wektorowe, zasoby technologiczne, kodowanie, algorytmy sztucznej inteligencji, zabezpieczenia systemów rządowych, systemy kontroli robotów, Aleksander Ragorski, nauczyciel angielskiego, związek, Kraków, ul. Bracka, automatyzacja, więź, walka, biegłość, krav maga, samoobrona, atak, konfrontacja, broń palna, niebezpieczny przeciwnik, teren miejski, koktajle Mołotowa, zamieszki, sabotaż, kontrasty, pragmatyzm, bezkompromisowość, pizza z ananasem, spokój, normalność, chaos, równowaga psychiczna, miejsce pobytu, działania, raporty, miasta o ograniczonym dostępie, projekt, komunikacja, jednostki SI",
    ["Aleksander Ragowski"] ="nauczyciel, język angielski, Szkoła Podstawowa nr 9, Grudziądz, kreatywne metody nauczania, zaangażowanie, społeczność szkolna, automatyzacja, rząd robotów, krytyk, nowy reżim, tajne spotkania, zagrożenia, kontrola edukacji, algorytmy, sztuczna inteligencja, aresztowanie, ucieczka, miejsce pobytu nieznane, obrzeża, strefy niedostępne, aktywność, programowanie, Java, stan psychiczny, opozycja, systemy rządowe, niebezpieczny wróg, zgłoszenie miejsca pobytu",
};

var reportsKeywords = new List<FileKeywords>();

foreach (var filePath in Directory.EnumerateFiles("pliki_z_fabryki").Where(x => Path.GetExtension(x) == ".txt"))
{
    var text = await File.ReadAllTextAsync(filePath);

    var prompt = $$"""
                   <persons>
                   {{string.Join(Environment.NewLine, personKeywords.Select(x => $"{x.Key}: {x.Value}"))}}
                   </persons>

                   <text>
                   {{text}}
                   </text>

                   Przygotuj listę słów kluczowych, które opisują tekst z taga "text" w formie mianownika w liczbie pojedynczej (czyli np. “sportowiec”, a nie “sportowcem”, “sportowców” itp.).
                   Zwróć uwagę na szczegóły i nie pomijaj żadnych informacji.
                   Spróbuj znaleźć osobę, którą opisuje tekst z taga "text" i znajdź o niej słowa kluczowe w tagu "persons".
                   Zwróć listę słów kluczowych z tagu "text" oraz z odnalezionej osoby jeśli to się udało.

                   <example_output>
                   ```json
                   {
                     textKeywords: ["słowo1", "słowo2", "słowo3"],
                     personKeywords: ["słowo4", "słowo5", "słowo6"]
                   }
                   ```
                   </example_output>
                   """;
    
    var clientResult = await chatClient.CompleteChatAsync([prompt], new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()});
    var response = JsonSerializer.Deserialize<Response>(clientResult.Value.Content[0].Text, JsonSerializerOptions.Web)!;
    var keywords = (response.TextKeywords ?? []).Union(response.PersonKeywords ?? []).Distinct();
    var fileKeywords = new FileKeywords(Path.GetFileName(filePath), string.Join(", ", keywords));
    reportsKeywords.Add(fileKeywords);
    
    Console.WriteLine($"""
                       File: {Path.GetFileName(filePath)}
                       Keywords: {fileKeywords.Keywords}
                       """);
    Console.WriteLine();
}

await PostAnswersAsync();

return;

async Task PostAnswersAsync()
{
    using var httpClient = new HttpClient();

    var requestBody = new
    {
        task = "dokumenty",
        apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
        answer = reportsKeywords.ToDictionary(x => x.Filename, x => x.Keywords),
    };

    var apiResponse = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
    
    Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");
}