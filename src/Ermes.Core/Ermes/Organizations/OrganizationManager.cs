using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.CompetenceAreas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Organizations
{
    public class OrganizationManager : DomainService
    {
        public IQueryable<Organization> Organizations { get { return OrganizationRepository.GetAll().Include(o => o.Parent).Include(o => o.Children); } }
        public IQueryable<OrganizationCompetenceArea> OrganizationCAs { get { return OrganizationCARepository.GetAll(); } }
        protected IRepository<Organization> OrganizationRepository { get; set; }
        protected IRepository<OrganizationCompetenceArea> OrganizationCARepository { get; set; }

        public OrganizationManager(IRepository<Organization> organizationRepository, IRepository<OrganizationCompetenceArea> organizationCARepository)
        {
            OrganizationRepository = organizationRepository;
            OrganizationCARepository = organizationCARepository;
        }

        public async Task<Organization> GetOrganizationByIdAsync(int id)
        {
            return await Organizations.SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Organization> GetOrganizationByNameAsync(string name)
        {
            return await Organizations.SingleOrDefaultAsync(a => a.Name == name);
        }

        public async Task<List<Organization>> GetAllAsync()
        {
            return await Organizations.ToListAsync();
        }

        public async Task<int> InsertOrganizationAsync(Organization org)
        {
            return await OrganizationRepository.InsertAndGetIdAsync(org);
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            var org = OrganizationRepository.Get(id);
            await OrganizationRepository.DeleteAsync(org);
        }

        public async Task<int> InsertOrganizationCompetenceAreaAsync(OrganizationCompetenceArea newItem)
        {
            return await OrganizationCARepository.InsertAndGetIdAsync(newItem);
        }

        //return the Ids of the table,
        //not the one related to competence areas
        public async Task<List<int>> GetOrganizationCompetenceAreaIdsByOrganizationId(int orgId)
        {
            return await OrganizationCAs
                            .Where(a => a.OrganizationId == orgId)
                            .Select(a => a.Id)
                            .ToListAsync();
        }

        public async Task DeleteOrganizationCompetenceAreaAsync(int id)
        {
            await OrganizationCARepository.DeleteAsync(id);
        }

        public int[] GetOrganizationIds()
        {
            return Organizations.Select(a => a.Id).ToArray();
        }

        public async Task<int[]> GetOrganizationIdsAsync()
        {
            return await Organizations.Select(a => a.Id).ToArrayAsync();
        }

        public async Task<bool> CheckParent(int? parentId)
        {
            if (parentId.HasValue)
            {
                var parent = await GetOrganizationByIdAsync(parentId.Value);
                return parent != null;
            }

            return true;
        }

        public async Task<Organization> GetParentOrganizationAsync(int? parentId)
        {
            if (parentId.HasValue)
                return await Organizations.SingleAsync(o => o.Id == parentId.Value);
            else
                return null;
        }
        public Organization GetParentOrganization(int? parentId)
        {
            return (parentId.HasValue ? Organizations.Single(o => o.Id == parentId.Value) : null);
        }
    }
}
