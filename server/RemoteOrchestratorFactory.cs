
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;

namespace Server
{
    public class RemoteOrchestratorFactory
    {
        public RemoteOrchestrator Create(SqlSyncChangeTrackingProvider provider)
        {
            var options = new SyncOptions();

            return new RemoteOrchestrator(provider, options);
        }
    }
}
