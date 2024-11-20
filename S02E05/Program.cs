using System.Net.Http.Json;
using HtmlAgilityPack;
using OpenAI.Audio;
using OpenAI.Chat;
using S02E05;

var questions = await GetQuestionsAsync();
var sections = await GetSectionsAsync();
var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var summary = await GetSectionSummaryAsync();
var answers = await GetAnswersAsync();
await PostAnswersAsync();

return;

async Task<List<List<HtmlNode>>> GetSectionsAsync()
{
    var web = new HtmlWeb();
    var htmlDocument = await web.LoadFromWebAsync("https://centrala.ag3nts.org/dane/arxiv-draft.html");
    var nodes = htmlDocument.DocumentNode.Descendants().First(node => node.HasClass("container")).ChildNodes;
    Console.WriteLine("Analysing the document...");
    List<List<HtmlNode>> list = [];
    foreach (var node in nodes)
    {
        if (node.NodeType != HtmlNodeType.Element)
        {
            continue;
        }
        if (node.Name is "h1" or "h2")
        {
            list.Add([node]);
        }
        else
        {
            list[^1].Add(node);
        }
    }
    Console.WriteLine("Document analysed.");
    return list;
}

async Task<string> GetSectionSummaryAsync()
{
    var partialSummary = "";

    foreach (var part in sections)
    {
        var heading = part[0].InnerText;
        List<ChatMessage> chatMessages =
        [
            ChatMessage.CreateSystemMessage(
                $"""
                Jesteś ekspertem w tworzeniu szczegółowych streszczeń na podstawie danego kontekstu.
                Podczas analizy dokumentu zwróć uwagę na kluczowe informacje, aby znaleźć odpowiedzi na pytania:
                {string.Join(", ", questions.Select(x => x.Text))}
                Posumowanie poprzednich części: {partialSummary}
                """
            ),
            ChatMessage.CreateUserMessage($"Nagłówek: {heading}"),
        ];

        for (var i = 1; i < part.Count; i++)
        {
            switch (part[i].Name)
            {
                case "p":
                    chatMessages.Add(ChatMessage.CreateUserMessage(part[i].InnerText));
                    break;
                case "figure":
                    var imageSrc = part[i].Descendants().First(node => node.Name == "img").Attributes["src"].Value;
                    var imagePart = ChatMessageContentPart.CreateImagePart(new Uri($"https://centrala.ag3nts.org/dane/{imageSrc}"));
                    var imageDescriptionResult = await chatClient.CompleteChatAsync(
                        "Opisz dokładnie obraz",
                        ChatMessage.CreateUserMessage(imagePart));
                    var imageCaption = part[i].Descendants().First(node => node.Name == "figcaption").InnerText;
                    chatMessages.Add(
                        $"""
                         Opis obrazka: {imageDescriptionResult.Value.Content[0].Text}
                         Podpis obrazka: {imageCaption}
                         """);
                    break;
                case "audio":
                {
                    var audioSrc = part[i].Descendants().First(node => node.Name == "source").Attributes["src"].Value;
                    var filename = audioSrc["i/".Length..];
                    {
                        using var httpClient = new HttpClient();
                        await using var audioStream =
                            await httpClient.GetStreamAsync($"https://centrala.ag3nts.org/dane/{audioSrc}");
                        await using var audioFile = new FileStream(filename, FileMode.Create);
                        await audioStream.CopyToAsync(audioFile);
                    }
                    {
                        await using var audioFile = File.OpenRead(filename);
                        var audioClient = new AudioClient("whisper-1", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
                        var audioTranscriptionResult = await audioClient.TranscribeAudioAsync(audioFile, filename);
                        chatMessages.Add($"Transkrypcja nagrania: {audioTranscriptionResult.Value.Text}");
                    }
                    break;
                }
            }
        }

        var sectionSummaryResult = await chatClient.CompleteChatAsync(chatMessages);
        partialSummary += sectionSummaryResult.Value.Content[0].Text;
    }
    
    Console.WriteLine();
    Console.WriteLine($"Podsumowanie: {partialSummary}");
    Console.WriteLine();
    
    return partialSummary;
}

async Task<List<Question>> GetQuestionsAsync()
{
    using var httpClient = new HttpClient();
    using var response = await httpClient.GetAsync($"https://centrala.ag3nts.org/data/{Environment.GetEnvironmentVariable("AIDEVS_KEY")}/arxiv.text");
    var text = await response.Content.ReadAsStringAsync();
    var lines = text.Split(Environment.NewLine);
    return lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new Question(x[..2], x[3..])).ToList();
}

async Task<List<Answer>> GetAnswersAsync()
{
    List<Answer> result = [];

    foreach (var question in questions)
    {
        var questionResult = await chatClient.CompleteChatAsync(
            ChatMessage.CreateSystemMessage(
                $"""
                 Jesteś ekspertem od odpowiadania krótko i jednym zdaniem na pytania.
                 Twoja wiedza:
                 {summary}
                 """),
            ChatMessage.CreateUserMessage(question.Text));

        result.Add(new Answer(question.Id, questionResult.Value.Content[0].Text));
            
        Console.WriteLine($"Question: {question}");
        Console.WriteLine($"Answer: {questionResult.Value.Content[0].Text}");
        Console.WriteLine();
    }

    return result;
}

async Task PostAnswersAsync()
{
    using var httpClient = new HttpClient();
    
    var apiResponse = await httpClient.PostAsJsonAsync(
        "https://centrala.ag3nts.org/report",
        new
        {
            task = "arxiv",
            apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
            answer = answers.ToDictionary(x => x.QuestionId, x => x.Text),
        });
    
    Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");
}

