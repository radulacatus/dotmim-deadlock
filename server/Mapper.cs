
using System.Linq;
using Server.Contract;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;

namespace Server
{
    public static class Mapper
    {
        public static SyncSetup MapToSyncSetup(this ProvisionScopeRequest request)
        {
            var tableNames = request.ColumnsPerTableStructure
               .Select(x => x.Key);
            var syncSetup = new SyncSetup(tableNames);
            foreach (var table in syncSetup.Tables)
            {
                table.SyncDirection = SyncDirection.DownloadOnly;
                table.Columns.AddRange(request.ColumnsPerTableStructure[table.TableName]);
            }

            foreach (var filter in request.SetupFilters)
            {
                var setupFilter = new SetupFilter(filter.TableName);
                setupFilter.CustomWheres.AddRange(filter.CustomWheres);
                syncSetup.Filters.Add(setupFilter);
            }

            return syncSetup;
        }
    }
}
