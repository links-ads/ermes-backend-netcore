using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Ermes.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<ErmesDbContext> dbContextOptions,
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for ErmesDbContext */
            //dbContextOptions.UseSqlServer(connectionString);
            dbContextOptions.UseNpgsql(connectionString,
                x => x.UseNetTopologySuite(geographyAsDefault: true)
            );
        }

        public static void Configure(DbContextOptionsBuilder<ErmesDbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection,
                x => x.UseNetTopologySuite(geographyAsDefault: true)
            );
        }
    }
}
