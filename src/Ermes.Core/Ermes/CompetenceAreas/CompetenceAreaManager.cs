using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.CompetenceAreas
{
    public class CompetenceAreaManager : DomainService
    {
        public IQueryable<CompetenceArea> CompetenceAreas { get { return CompetenceAreaRepository.GetAll(); } }
        protected IRepository<CompetenceArea> CompetenceAreaRepository { get; set; }

        public CompetenceAreaManager(IRepository<CompetenceArea> competenceAreaRepository)
        {
            CompetenceAreaRepository = competenceAreaRepository;
        }

        public int InsertCompetenceArea(CompetenceArea compArea)
        {
            return CompetenceAreaRepository.InsertAndGetId(compArea);
        }

        public Dictionary<string, List<string>> GetUuidDictionary()
        {
            return CompetenceAreas
                    .Select(a => new { a.Source, a.Uuid})
                    .AsEnumerable()
                    .GroupBy(a => a.Source)
                    .ToDictionary(a => a.Key, b => b.Select(c => c.Uuid).ToList());
        }

        public async Task<bool> CheckIfCompetenceAreaIdExists(int compAreaId)
        {
            return await CompetenceAreas
                            .Select(a => a.Id)
                            .CountAsync(a => a == compAreaId) > 0;
           
        }
    }
}
