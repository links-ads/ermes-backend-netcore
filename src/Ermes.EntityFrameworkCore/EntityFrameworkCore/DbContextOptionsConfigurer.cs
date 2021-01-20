using Microsoft.EntityFrameworkCore;

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
            dbContextOptions.UseSqlServer(connectionString);
        }
    }
}
