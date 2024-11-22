using Neo4j.Driver;
using static S03E05.Helper;

await using var driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "Test123!"));
await using var session = driver.AsyncSession();
await SeedDatabaseAsync(session);

var shortestPath = await session.ExecuteReadAsync(async tx =>
{
    const string shortestPathQuery = """
                                     MATCH (start:User { username: $startName }),
                                           (end:User { username: $endName }),
                                            path = shortestPath((start)-[:CONNECTED_TO*]-(end))
                                     RETURN [node in nodes(path) | node.username] as usernames,
                                            length(path) as pathLength
                                     """;

    var result = await tx.RunAsync(shortestPathQuery,
        new { startName = "Rafał", endName = "Barbara" });

    var record = await result.SingleAsync();
    var usernames = record["usernames"].As<List<string>>();
    var pathLength = record["pathLength"].As<int>();

    Console.WriteLine(@"Najkrótsza ścieżka od ""Rafał"" do ""Barbara"":");
    Console.WriteLine($"Długość ścieżki: {pathLength} połączeń");
    Console.WriteLine($"Ścieżka: {string.Join(" -> ", usernames)}");

    return usernames;
});

await PostAnswersAsync(string.Join(',', shortestPath));

return;

async Task SeedDatabaseAsync(IAsyncSession asyncSession)
{
    var users = await QueryDatabaseAsync<User>("select * from users");
    var connections = await QueryDatabaseAsync<Connection>("select * from connections");

    await asyncSession.ExecuteWriteAsync(async work =>
    {
        foreach (var user in users)
        {
            await work.RunAsync(
                "MERGE (u:User { id: $id, username: $username })",
                new { id = user.Id, username = user.Username });
        }

        foreach (var connection in connections)
        {
            var createConnectionQuery = await work.RunAsync(
                """
                MATCH (u1:User { id: $id1 })
                MATCH (u2:User { id: $id2 })
                MERGE (u1)-[:CONNECTED_TO]->(u2)
                RETURN u1.username, u2.username
                """,
                new { id1 = connection.User1Id, id2 = connection.User2Id });

            var record = await createConnectionQuery.SingleAsync();
        }
    });
}

public sealed record User(string Id, string Username, string AccessLevel, string IsActive, DateOnly LastLog);

public sealed record Connection(string User1Id, string User2Id);