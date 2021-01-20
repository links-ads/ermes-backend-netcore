using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ermes.EntityFrameworkCore
{
    public class ErmesDbContext : AbpDbContext
    {
        //Add DbSet properties for your entities...

        public ErmesDbContext(DbContextOptions<ErmesDbContext> options) 
            : base(options)
        {

        }
    }
}
