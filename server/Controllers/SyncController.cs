using System.Linq;
using System.Threading.Tasks;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Mvc;
using Server.Contract;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly SqlSyncProviderFactory sqlSyncProviderFactory;
        private readonly RemoteOrchestratorFactory remoteOrchestratorFactory;

        public SyncController(
        SqlSyncProviderFactory sqlSyncProviderFactory,
        RemoteOrchestratorFactory remoteOrchestratorFactory)
        {
            this.sqlSyncProviderFactory = sqlSyncProviderFactory;
            this.remoteOrchestratorFactory = remoteOrchestratorFactory;
        }

        [HttpPost]
        public async Task Post([FromHeader] string scopeName)
        {
            var provider = sqlSyncProviderFactory.CreateTrackingProvider();
            var orchestrator = remoteOrchestratorFactory.Create(provider);

            var scopeInfos = await orchestrator.GetAllScopeInfosAsync();
            var existingScope = scopeInfos.FirstOrDefault(x => x.Name == scopeName);
            if (existingScope == null)
            {
                return;
            }
            
            var webServerAgent = new WebServerAgent(provider, existingScope.Setup, scopeName: scopeName);
            await webServerAgent.HandleRequestAsync(this.HttpContext);
        }

        [HttpPost]
        [Route("scope")]
        public async Task<IActionResult> ProvisionScope([FromBody] ProvisionScopeRequest request)
        {
            var provider = sqlSyncProviderFactory.CreateTrackingProvider();
            var orchestrator = remoteOrchestratorFactory.Create(provider);

            var scopeInfos = await orchestrator.GetAllScopeInfosAsync();
            var existingScope = scopeInfos.FirstOrDefault(x => x.Name == request.ScopeName);
            if (existingScope != null)
            {
                var p = SyncProvision.StoredProcedures | SyncProvision.TrackingTable |
                    SyncProvision.Triggers;

                await orchestrator.DeprovisionAsync(request.ScopeName, p);
                await orchestrator.DeleteScopeInfoAsync(existingScope);
            }

            await orchestrator.ProvisionAsync(request.ScopeName, request.MapToSyncSetup());

            return Ok();
        }
    }
}