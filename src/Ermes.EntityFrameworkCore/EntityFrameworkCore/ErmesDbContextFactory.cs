using Ermes.Configuration;
using Ermes.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ermes.EntityFrameworkCore
{
    /* This class is needed to run EF Core PMC commands. Not used anywhere else */
    public class ErmesDbContextFactory : IDesignTimeDbContextFactory<ErmesDbContext>
    {
        public ErmesDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ErmesDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString(ErmesConsts.ConnectionStringName)
            );

            return new ErmesDbContext(builder.Options, null);
        }
    }
}