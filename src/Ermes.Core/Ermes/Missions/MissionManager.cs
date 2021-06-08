using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Ermes.Enums;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
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

        public bool CheckNewStatus(MissionStatusType currentStatus, MissionStatusType newStatus)
        {
            return currentStatus switch
            {
                MissionStatusType.Created => newStatus == MissionStatusType.TakenInCharge || newStatus == MissionStatusType.Deleted,
                MissionStatusType.TakenInCharge => newStatus == MissionStatusType.Created || newStatus == MissionStatusType.Deleted || newStatus == MissionStatusType.Completed,
                _ => false,
            };
        }

        public async Task<Mission> GetMissionByIdAsync(int missionId)
        {
            return await Missions.SingleOrDefaultAsync(m => m.Id == missionId);
        }
        public Mission GetMissionById(int missionId)
        {
            return Missions.SingleOrDefault(m => m.Id == missionId);
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

        public IQueryable<Mission> GetMissions(DateTime startDate, DateTime endDate)
        {
            NpgsqlRange<DateTime> range = new NpgsqlRange<DateTime>(startDate, endDate);
            return Missions
                    .Where(m => m.Duration.Overlaps(range));
        }

    }
}
