using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Gamification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    public class GamificationManager: DomainService
    {
        protected IRepository<Level> LevelRepositiry{ get; set; }
        public IQueryable<Level> Levels { get { return LevelRepositiry.GetAll(); } }

        public GamificationManager(IRepository<Level> levelRepositiry)
        {
            LevelRepositiry = levelRepositiry;
        }

        public async Task<List<Level>> GetLevelsAsync()
        {
            return await Levels.ToListAsync();
        }
    }
}
