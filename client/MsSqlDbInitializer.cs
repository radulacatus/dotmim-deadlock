namespace DataSyncClient
{
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;

    public class MsSqlDbInitializer
    {
        private readonly string connectionString;
        private readonly string masterConnectionString;
        private readonly string databaseName;

        public MsSqlDbInitializer(
            string connectionString)
        {
            this.connectionString = connectionString;

            var builder = new SqlConnectionStringBuilder(connectionString);
            databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";
            masterConnectionString = builder.ConnectionString;
        }

        public async Task ReinitializeDb()
        {
            if (await IsDbExisting())
            {
                await DropDb();
            }

            await CreateDb();

            await ActivateChangeTrackingOnDb();
        }

        public async Task ActivateChangeTrackingOnDb()
        {
            if (!await IsDbChangeTrackingActive())
            {
                await ActivateDbChangeTracking();
            }
        }

        private async Task ActivateDbChangeTracking()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("ALTER DATABASE CURRENT SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 14 DAYS, AUTO_CLEANUP = ON)", conn))
                {
                    conn.Open();

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<bool> IsDbChangeTrackingActive()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(ctd.database_id) FROM sys.change_tracking_databases ctd " +
                    "INNER JOIN sys.databases sd ON ctd.database_id = sd.database_id " +
                    "WHERE name = DB_NAME()";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    return (int)await cmd.ExecuteScalarAsync() == 1;
                }
            }
        }

        private async Task<bool> IsDbExisting()
        {
            using SqlConnection conn = new SqlConnection(masterConnectionString);
            using var cmd = new SqlCommand("SELECT COUNT(name) FROM master.dbo.sysdatabases WHERE name = @database", conn);
            
            cmd.Parameters.Add("@database", SqlDbType.NVarChar).Value = databaseName;
            conn.Open();

            return (int)await cmd.ExecuteScalarAsync() == 1;
        }

        private async Task DropDb()
        {
            await ExecuteNonQueryAsync($"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE DROP DATABASE {databaseName}");

            await AwaitDbDropped();
        }

        private async Task AwaitDbDropped()
        {
            var dbExists = await IsDbExisting();

            if (dbExists)
            {
                throw new Exception("Database still exists after dropping");
            }
        }

        private async Task CreateDb() => ExecuteNonQueryAsync($"CREATE DATABASE {databaseName}");

        private async Task ExecuteNonQueryAsync(string cmdText)
        {
            using (var conn = new SqlConnection(masterConnectionString))
            {
                using (var cmd = new SqlCommand(cmdText, conn))
                {
                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
