using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repo;
using System;

[assembly: FunctionsStartup(typeof(MatchDataRequester.Startup))]
namespace MatchDataRequester
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            FunctionConfigHelper.InitialiseConfigBuilder();

            var configString = FunctionConfigHelper.GetConnectionString("ConnString");

            builder.Services.AddDbContext<LolContext>(options => options.UseSqlServer(configString));
            builder.Services.AddHttpClient();
        }
    }
}
