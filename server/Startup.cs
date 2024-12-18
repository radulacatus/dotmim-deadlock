using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>options.IdleTimeout = TimeSpan.FromMinutes(3));

            services.AddScoped<RemoteOrchestratorFactory>();
            services.AddScoped<SqlSyncProviderFactory>();
            // var connectionString = this.Configuration.GetSection("ConnectionStrings")["SqlConnection"];
            // var tables = new string[]
            // {
            //     "Customer", "CustomerAddress", 
            // };

            // var webServerOptions = new WebServerOptions();
            // var options = new SyncOptions { };
            // var provider = new SqlSyncChangeTrackingProvider(connectionString);
            // services.AddSyncServer(provider, tables, options, webServerOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}