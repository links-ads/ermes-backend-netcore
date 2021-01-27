using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Missions
{
    public class MissionManager : DomainService
    {
        public IQueryable<Mission> Missions { get { return MissionRepository.GetAll().Include(a => a.CreatorPerson.Organization); } }
        protected IRepository<Mission> MissionRepository { get; set; }
        protected IRepository<Person, long> PersonRepository { get; set; }

        public MissionManager(
                IRepository<Mission> missionRepository,
                IRepository<Person, long> personRepository
            )
        {
            MissionRepository = missionRepository;
            PersonRepository = personRepository;
        }

        public async Task<Mission> GetMissionByIdAsync(int missionId)
        {
            return await Missions.SingleOrDefaultAsync(m => m.Id == missionId);
        }

        public async Task<int> InsertMissionAsync(Mission mission)
        {
            return await MissionRepository.InsertAndGetIdAsync(mission);
        }

        public IQueryable<Person> GetMissionCoordinators(long? CoordinatorPersonId, int? CoordinatorTeamId, int? OrganizationId)
        {
            if (CoordinatorPersonId.HasValue)
                return PersonRepository.GetAll().Where(p => p.Id == CoordinatorPersonId);
            else if (CoordinatorTeamId.HasValue)
                return PersonRepository.GetAll().Where(p => p.TeamId == CoordinatorTeamId);
            else
                return PersonRepository.GetAll().Where(p => p.OrganizationId == OrganizationId);
        }

        public List<Mission> GetCurrentMissions(IPersonBase person)
        {
            return MissionRepository.GetAll()
                .Where(m => m.CoordinatorPersonId == person.Id || m.CoordinatorTeamId == person.TeamId || (!m.CoordinatorPersonId.HasValue && !m.CoordinatorTeamId.HasValue && m.OrganizationId == person.OrganizationId))
                .Where(m => m.CurrentStatusString == Enums.MissionStatusType.Created.ToString() || m.CurrentStatusString == Enums.MissionStatusType.TakenInCharge.ToString())
                .ToList();
        }

    }
}
