using System.Net.Http.Json;
using System.Text.Json;
using OpenAI.Images;

using var httpClient = new HttpClient();

var robotIdResponseBody = await httpClient.GetFromJsonAsync<RobotIdResponseBody>($"https://centrala.ag3nts.org/data/{Environment.GetEnvironmentVariable("AIDEVS_KEY")}/robotid.json", JsonSerializerOptions.Web);
Console.WriteLine($"API response: {robotIdResponseBody!.Description}");

var imageClient = new ImageClient("dall-e-3", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

var generateImageAsync = await imageClient.GenerateImageAsync(
    robotIdResponseBody.Description, 
    new ImageGenerationOptions
    {
        ResponseFormat = GeneratedImageFormat.Uri,
        Size = GeneratedImageSize.W1024xH1024,
        Style = GeneratedImageStyle.Vivid,
    });

Console.WriteLine($"Image URI: {generateImageAsync.Value.ImageUri}");

var apiResponse = await httpClient.PostAsJsonAsync(
    "https://centrala.ag3nts.org/report",
    new
    {
        task = "robotid",
        apikey = Environment.GetEnvironmentVariable("AIDEVS_KEY"),
        answer = generateImageAsync.Value.ImageUri,
    });

Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");
