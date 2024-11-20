namespace S02E05;

public sealed record Question(string Id, string Text);

public sealed record Answer(string QuestionId, string Text);