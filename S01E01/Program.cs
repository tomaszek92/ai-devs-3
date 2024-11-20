using HtmlAgilityPack;
using OpenAI.Chat;

var web = new HtmlWeb();
var htmlDocument = await web.LoadFromWebAsync("https://xyz.ag3nts.org/");
var humanQuestionHtmlNode = htmlDocument.GetElementbyId("human-question");
var humanQuestion = humanQuestionHtmlNode.InnerText["Question:".Length..];
Console.WriteLine(humanQuestion);

var question = $"{humanQuestion} Podaj tylko rok.";
var client = new ChatClient(
    "gpt-4o",
    Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var humanQuestionChatResult = await client.CompleteChatAsync(question);
var year = humanQuestionChatResult.Value.Content[0].Text;
Console.WriteLine(year);

using var httpClient = new HttpClient();
var content = new FormUrlEncodedContent(new Dictionary<string, string>
{
    { "username", "tester" },
    { "password", "574e112a" },
    { "answer", year },
});
var response = await httpClient.PostAsync($"https://xyz.ag3nts.org/", content);
var responseContent = await response.Content.ReadAsStringAsync();
Console.WriteLine(responseContent);