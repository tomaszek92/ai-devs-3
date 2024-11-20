using System.Net.Http.Json;
using OpenAI.Embeddings;
using Qdrant.Client;
using Qdrant.Client.Grpc;

const string CollectionName = "s_03_e_02";

using var qdrantClient = new QdrantClient(
    new Uri("https://7c9e3a19-47a6-4bdc-8d12-fff309fb5368.us-east4-0.gcp.cloud.qdrant.io:6334"),
    apiKey: Environment.GetEnvironmentVariable("QDRANT_API_KEY"));

await qdrantClient.RecreateCollectionAsync(CollectionName, new VectorParams { Size = 3072, Distance = Distance.Cosine });

var embeddingClient = new EmbeddingClient("text-embedding-3-large", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
await UpsertEmbeddingsAsync();

const string question = "W raporcie, z którego dnia znajduje się wzmianka o kradzieży prototypu broni?";
var questionEmbeddings = await embeddingClient.GenerateEmbeddingAsync(question);
var scoredPoints = await qdrantClient.QueryAsync(CollectionName, questionEmbeddings.Value.ToFloats().ToArray(), limit: 1);
var responseValue = scoredPoints[0].Payload["date"];
Console.WriteLine($"Answer: {responseValue.StringValue}, Score: {scoredPoints[0].Score}");

await PostAnswersAsync(responseValue.StringValue);

return;

async Task UpsertEmbeddingsAsync()
{
    foreach (var filePath in Directory.EnumerateFiles("do-not-share"))
    {
        var text = await File.ReadAllTextAsync(filePath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        var date = DateOnly.ParseExact(fileNameWithoutExtension, "yyyy_MM_dd");
        var embeddings = await embeddingClient.GenerateEmbeddingAsync(text);

        var updateResult = await qdrantClient.UpsertAsync(
            CollectionName,
            [
                new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = embeddings.Value.ToFloats().ToArray(),
                    Payload =
                    {
                        { "date", date.ToString("yyyy-MM-dd") },
                    },
                },
            ]
        );

        if (updateResult.Status == UpdateStatus.Completed)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Upserted {Path.GetFileName(filePath)}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Failed to upsert {Path.GetFileName(filePath)}");
        }
        Console.ResetColor();
    }
}

async Task PostAnswersAsync(string response)
{
    using var httpClient = new HttpClient();

    var requestBody = new
    {
        task = "wektory",
        apikey = new Guid(Environment.GetEnvironmentVariable("AIDEVS_KEY")!),
        answer = response,
    };

    var apiResponse = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", requestBody);
    
    Console.WriteLine($"API response: {await apiResponse.Content.ReadAsStringAsync()}");
}