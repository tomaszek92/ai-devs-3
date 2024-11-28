using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using S04E04;
using S04E04.Requests;
using S04E04.Responses;
using Serilog;
using ILogger = Serilog.ILogger;
using static S04E04.Shared;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY")!));
builder.Services.AddHttpClient();
builder.Services.AddOpenApi();
builder.Services.AddSerilog(loggerConfiguration =>
    loggerConfiguration.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost(
    "/api/start",
    async (
        [FromBody] StartRequest startRequest,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] ILogger logger
    ) =>
    {
        var httpClient = httpClientFactory.CreateClient("centrala");
        var request = new
        {
            apikey = Environment.GetEnvironmentVariable("AIDEVS_KEY"),
            answer = startRequest.ApiUrl,
            task = "webhook",
        };
        using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://centrala.ag3nts.org/report", request);
        var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
        logger.Information($"API response: {responseJson}");
    });

app.MapPost(
    "/api/description",
    async (
        [FromBody] DescriptionRequest request,
        [FromServices] ChatClient chatClient,
        [FromServices] ILogger logger
    ) =>
    {
        logger.Information("Received instruction: {Instruction}", request.Instruction);

        var chatResponse = await chatClient.CompleteChatAsync(
            [
                ChatMessage.CreateSystemMessage(SystemPrompt),
                ChatMessage.CreateUserMessage(request.Instruction)
            ],
            new ChatCompletionOptions { ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() }
        );

        var json = chatResponse.Value.Content[0].Text;
        var chatCoordinatesResponse = JsonSerializer.Deserialize<ChatCoordinatesResponse>(json, JsonSerializerOptions.Web)!;
        logger.Information("Chat coordinates response: {ChatCoordinatesResponse}", chatCoordinatesResponse);
        var description = Descriptions[(chatCoordinatesResponse.Row, chatCoordinatesResponse.Column)];
        logger.Information("Description: {Description}", description);

        return TypedResults.Ok(new { description});
    });

app.Run();