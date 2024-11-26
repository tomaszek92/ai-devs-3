using System.Text.Json;
using OpenAI.Files;
using OpenAI.FineTuning;
using S04E02;

var correctLines = await File.ReadAllLinesAsync(Path.Combine("lab_data", "correct.txt"));
var correctMessages = PrepareFineTuningData(correctLines, "YES");

var incorrectLines = await File.ReadAllLinesAsync(Path.Combine("lab_data", "incorrect.txt"));
var incorrectMessages = PrepareFineTuningData(incorrectLines, "NO");

var messageGroups = correctMessages
    .Union(incorrectMessages)
    .OrderBy(_ => Random.Shared.Next())
    .Select(x => JsonSerializer.Serialize(x, JsonSerializerOptions.Web))
    .ToArray();

await File.WriteAllLinesAsync("testing_data_all.jsonl", messageGroups);

return;

static MessageGroup[] PrepareFineTuningData(string[] lines, string label, int startIndex = 0, int? count = null) =>
    lines
        .Skip(startIndex)
        .Take(count ?? lines.Length - startIndex)
        .Select(line =>
            new MessageGroup(
            [
                new Message("system", "validate numbers"),
                new Message("user", line),
                new Message("assistant", label),
            ])
        )
        .ToArray();