using System.Net.Http.Json;
using Flurl.Http;

var allLines = await "https://poligon.aidevs.pl/dane.txt".GetStringAsync();
var lines = allLines.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
var request = new Request("POLIGON", Environment.GetEnvironmentVariable("AIDEVS_KEY")!, lines);

using var httpClient = new HttpClient();
using var httpResponseMessage = await httpClient.PostAsJsonAsync("https://poligon.aidevs.pl/verify", request);
var response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();

Console.WriteLine(response);

file sealed record Request(string Task, string Apikey, string[] Answer);

file sealed record Response(int Code, string Message);