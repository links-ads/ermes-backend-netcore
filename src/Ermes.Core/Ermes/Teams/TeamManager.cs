using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Teams
{
    public class TeamManager:DomainService
    {
        protected IRepository<Team> TeamRepository { get; set; }
        public IQueryable<Team> Teams { get { return TeamRepository.GetAll(); } }

        public TeamManager(IRepository<Team> teamRepository)
        {
            TeamRepository = teamRepository;
        }

        public virtual async Task DeleteTeamAsync(int TeamId)
        {
            await TeamRepository.DeleteAsync(TeamId);
        }

        public virtual Team GetTeamById(int TeamId)
        {
            return TeamRepository.GetAllIncluding(t=>t.Organization).FirstOrDefault(t=>t.Id==TeamId);
        }

        public virtual async Task<Team> GetTeamByIdAsync(int TeamId)
        {
            return await TeamRepository.FirstOrDefaultAsync(TeamId);
        }

        public virtual async Task<int> InsertTeamAsync(Team team)
        {
            return await TeamRepository.InsertAndGetIdAsync(team);
        }

        public virtual async Task<Team> GetTeamByNameAndOrganizationIdAsync(string teamName, int organizationId)
        {
            return await Teams.SingleOrDefaultAsync(t => t.Name == teamName && t.OrganizationId == organizationId);
        }
    }
}
