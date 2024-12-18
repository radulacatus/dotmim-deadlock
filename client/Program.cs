using DataSyncClient;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Web.Client;

const string clientConnectionString = $"Data Source=localhost;Initial Catalog=ClientDb;Integrated Security=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

string schemaWithDateAndTombstoneFilter(string scope) => $"{{\"ScopeName\":\"{scope}\", \"ColumnsPerTableStructure\":{{\"Customer\":[\"CustomerID\",\"FirstName\",\"LastName\",\"rowguid\"], \"CustomerAddress\":[\"AddressID\",\"CustomerID\",\"AddressLine1\",\"rowguid\",\"ValidUntil\"]}},\"SetupFilters\": [{{\"TableName\": \"CustomerAddress\", \"CustomWheres\":[\"ValidUntil > getdate() OR [side].[sync_row_is_tombstone] = 1\"]}}]}}";

async Task ProvisionScopeAsync(string scope, Func<string, string> schemaFactory, string connectionString)
{
    await new MsSqlDbInitializer(connectionString).ReinitializeDb();

    var httpClient = new HttpClient();
    var content = new StringContent(schemaFactory(scope), System.Text.Encoding.UTF8, "application/json");
    await httpClient.PostAsync("https://localhost:44342/api/sync/scope", content);
}

async Task SynchronizeAsync(string scope, string connStr)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("scopeName", scope);
    var serverProxyOrchestrator = new WebRemoteOrchestrator("https://localhost:44342/api/sync", client: client);
    var clientProvider = new SqlSyncProvider(connStr);
    var progress = new ConsoleLogProgress();
    var agent = new SyncAgent(clientProvider, serverProxyOrchestrator);
    await agent.SynchronizeAsync(scope, syncType: Dotmim.Sync.Enumerations.SyncType.Reinitialize, progress);

    while (true)
    {
        if (Console.KeyAvailable)
        {
            var key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.Q)
            {
                Console.WriteLine("\n'q' key pressed. Exiting...");
                break;
            }
        }

        try
        {
            var s1 = await agent.SynchronizeAsync(scope, syncType: Dotmim.Sync.Enumerations.SyncType.Normal, progress);
            Console.WriteLine(s1);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            await Task.Delay(1000);
        }
    }
}

var scope = Guid.NewGuid().ToString().Substring(0, 4);
await ProvisionScopeAsync(scope, schemaWithDateAndTombstoneFilter, clientConnectionString);
Console.WriteLine($"Provisioned scope {scope}");
await SynchronizeAsync(scope, clientConnectionString);
Console.WriteLine($"Stopped sync scope {scope}");
