using OpenAI.Chat;

const string prompt = """
                      You are an expert at Polish geography, topography, architecture and history.
                      You are looking at different parts of a map of a city in Poland and one part is from different city.
                      Identify the city and provide thinking process.
                      """;

await using var stream = File.OpenRead("map.jpeg");
var imageBinaryData = await BinaryData.FromStreamAsync(stream);
var chatMessageContentPart = ChatMessageContentPart.CreateImagePart(imageBinaryData, "image/jpeg");
var chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var cityResult = await chatClient.CompleteChatAsync(
    ChatMessage.CreateUserMessage(prompt),
    ChatMessage.CreateUserMessage(chatMessageContentPart)
);

Console.WriteLine($"City name: {cityResult.Value.Content[0].Text}");