using OpenAI.Chat;
using S04E01;
using static S04E01.Helper;
using Action = S04E01.Action;

const int MaxActionPerImage = 5;

var reportEndpointResponse = await TalkWithReportEndpointAsync("START");

var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var aiResponse = await InterpretAsync(chatClient, reportEndpointResponse);

Console.WriteLine($"AI response: {aiResponse}");

var imageUris = aiResponse.Split(Environment.NewLine).Select(x => new Uri(x.Trim())).ToArray();

foreach (var imageUri in imageUris)
{
    Console.WriteLine();
    Console.WriteLine($"Processing {imageUri}");

    var uri = imageUri;
    var action = string.Empty;
    for (var i = 1; i <= MaxActionPerImage; i++)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            action = await chatClient.AskAiAsync(uri, $"""
                                                           Jesteś ekspertem od analizy zdjęć.
                                                           Twoim zadaniem jest zidentyfikowanie kobiety.
                                                           Jeśli nie możesz zidentyfikować kobiety, możesz podjąć jedną z następujących akcji:
                                                           - {Action.Repair} - jeśli nie możesz zidentyfikować ludzi na zdjęciu
                                                           - {Action.Darken}
                                                           - {Action.Brighten}
                                                           Jeśli link nie zawiera zdjęcia, to zwróć {Action.Failure}.
                                                           Jeśli udało się Tobie rozpoznać kobietę, zwróć {Action.Success}.
                                                           """);
        }
        else if (action == Action.Success)
        {
            Console.WriteLine($"{Action.Success} for {uri}");

            aiResponse = await chatClient.AskAiAsync(uri, "Opisz kobietę na zdjęciu");
            Console.WriteLine($"AI response: {aiResponse}");
            reportEndpointResponse = await TalkWithReportEndpointAsync(aiResponse);
            Console.WriteLine($"Report API response: {reportEndpointResponse}");

            break;
        }
        else if (action == Action.Failure)
        {
            Console.WriteLine($"{Action.Failure} for {uri}");
            break;
        }
        else
        {
            Console.WriteLine($"{action} for {uri}");
            reportEndpointResponse = await TalkWithReportEndpointAsync($"{action} {Path.GetFileName(uri.LocalPath)}");
            aiResponse = await InterpretAsync(chatClient, reportEndpointResponse);
            Console.WriteLine($"AI response: {aiResponse}");
            if (!Uri.TryCreate(aiResponse.Trim(), UriKind.RelativeOrAbsolute, out uri))
            {
                break;
            }

            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri("https://centrala.ag3nts.org/dane/barbara/" + uri);
            }

            action = string.Empty;
        }
    }
}

return;

async Task<string> InterpretAsync(ChatClient chatClient1, ReportEndpointResponse reportEndpointResponse1)
{
    return await chatClient1.AskAiAsync($"""
                                         Na podstawie wiadomości, wyciągnij informacje o linkach url lub nazwie pliku.
                                         <wiadomość>
                                         {reportEndpointResponse1.Message}
                                         </wiadomość>
                                         <output>
                                         Zwróć same linki do zdjęć- każdy z linków w nowym wierszu.
                                         </output>
                                         """);
}