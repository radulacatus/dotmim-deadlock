
using Dotmim.Sync.SqlServer;

namespace Server
{
    public class SqlSyncProviderFactory
    {
        public SqlSyncChangeTrackingProvider CreateTrackingProvider()
        {
            const string connectionString = "Data Source=localhost;Initial Catalog=ServerDb;Integrated Security=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";
            return new SqlSyncChangeTrackingProvider(connectionString);
        }
    }
}
