using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Ermes.Linq.Extensions;
using System.Threading.Tasks;
using Ermes.Roles;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Organizations;

namespace Ermes.Persons
{
    public class PersonManager : DomainService
    {
        protected IRepository<Person, long> PersonRepository { get; set; }
        protected IRepository<PersonAction> PersonActionsRepository { get; set; }
        protected IRepository<PersonRole> PersonRoleRepository { get; set; }
        protected IRepository<PersonTip> PersonTipRepository { get; set; }
        protected IRepository<PersonQuiz> PersonQuizRepository { get; set; }
        protected IRepository<Role> RolesRepository { get; set; }
        protected IRepository<Organization> OrganizationsRepository { get; set; }

        public IQueryable<Person> Persons { get { return PersonRepository.GetAll().Include(a => a.Organization).Include(p => p.Team).Include(p => p.Level); } }
        public IQueryable<Role> Roles { get { return RolesRepository.GetAll(); } }
        public IQueryable<PersonAction> PersonActions { get { return PersonActionsRepository.GetAll(); } }
        public IQueryable<PersonRole> PersonRoles { get { return PersonRoleRepository.GetAll().Include(pr => pr.Role); } }
        public IQueryable<PersonTip> PersonTips { get { return PersonTipRepository.GetAll(); } }
        public IQueryable<PersonQuiz> PersonQuizzes { get { return PersonQuizRepository.GetAll(); } }
        public IQueryable<Organization> Organizations { get { return OrganizationsRepository.GetAll(); } }

        public PersonManager(IRepository<Person, long> personRepository,
                                IRepository<PersonAction> personActionsRepository,
                                IRepository<PersonRole> personRoleRepository,
                                IRepository<Role> rolesRepository,
                                IRepository<PersonTip> personTipRepository,
                                IRepository<PersonQuiz> personQuizRepository,
                                IRepository<Organization> organizationsRepository)
        {
            PersonRepository = personRepository;
            PersonActionsRepository = personActionsRepository;
            PersonRoleRepository = personRoleRepository;
            RolesRepository = rolesRepository;
            PersonTipRepository = personTipRepository;
            OrganizationsRepository = organizationsRepository;
            PersonQuizRepository = personQuizRepository;
        }

        public async Task<Person> GetPersonByIdAsync(long personId)
        {
            return await Persons.SingleOrDefaultAsync(p => p.Id == personId);
        }

        public async Task<Person> GetPersonByFusionAuthUserGuidAsync(Guid userId, string email, string username)
        {
            var person = await Persons.FirstOrDefaultAsync(a => a.FusionAuthUserGuid == userId);
            if (person == null)
            {
                if (userId.CompareTo(Guid.Empty) == 0)
                    throw new UserFriendlyException(L("InvalidGuid"));
                person = new Person()
                {
                    FusionAuthUserGuid = userId,
                    Username = username,
                    Email = email
                };
                person.Id = PersonRepository.InsertAndGetId(person);
            }

            return person;
        }


        //See issue #48
        //By managing in this way the unit of work, the ErmesDbContex is not disposed
        // and we can retrieve the person correctly
        public async Task<Person> GetPersonByFusionAuthUserGuidAsync(Guid userId)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var person = await Persons.FirstOrDefaultAsync(a => a.FusionAuthUserGuid == userId);
                if (person == null)
                {
                    if (userId.CompareTo(Guid.Empty) == 0)
                        throw new UserFriendlyException(L("InvalidGuid"));
                    person = new Person()
                    {
                        FusionAuthUserGuid = userId
                    };
                    person.Id = PersonRepository.InsertAndGetId(person);
                    person.IsNewUser = true;
                }
                uow.Complete();

                return person;
            }
        }

        public Person GetPersonByFusionAuthUserGuid(Guid userId)
        {
            var person = Persons.FirstOrDefault(a => a.FusionAuthUserGuid == userId);
            if (person == null)
            {
                if (userId.CompareTo(Guid.Empty) == 0)
                    throw new UserFriendlyException(L("InvalidGuid"));
                person = new Person()
                {
                    FusionAuthUserGuid = userId
                };
                person.Id = PersonRepository.InsertAndGetId(person);
            }

            return person;
        }

        public async Task<int> InsertPersonActionSharingPositionAsync(PersonActionSharingPosition item)
        {
            return await PersonActionsRepository.InsertAndGetIdAsync(item);
        }

        public async Task<int> InsertPersonActionTrackingAsync(PersonActionTracking item)
        {
            return await PersonActionsRepository.InsertAndGetIdAsync(item);
        }
        public async Task<int> InsertPersonActionStatusAsync(PersonActionStatus item)
        {
            return await PersonActionsRepository.InsertAndGetIdAsync(item);
        }
        public async Task<int> InsertPersonActionActivityAsync(PersonActionActivity item)
        {
            return await PersonActionsRepository.InsertAndGetIdAsync(item);
        }

        public async Task<List<PersonActionTracking>> GetPersonsActionTracking(IPersonBase person, int organizationId, DateTime start, DateTime end)
        {
            var orgList = organizationId > 0 ? new List<int>() { organizationId } : null;

            return await PersonActions
                            .DataOwnership(orgList, person)
                            .Where(a => a.Timestamp >= start && a.Timestamp <= end)
                            .OfType<PersonActionTracking>()
                            .Include(a => a.Person.Team)
                            .ToListAsync();
        }
        public async Task<List<PersonActionStatus>> GetPersonsActionStatus(IPersonBase person, int organizationId, DateTime start, DateTime end)
        {
            var orgList = organizationId > 0 ? new List<int>() { organizationId } : null;

            return await PersonActions
                    .DataOwnership(orgList, person)
                    .Where(a => a.Timestamp >= start && a.Timestamp <= end)
                    .OfType<PersonActionStatus>()
                    .Include(a => a.Person.Team)
                    .ToListAsync();
        }
        public async Task<List<PersonActionActivity>> GetPersonsActionActivity(IPersonBase person, int organizationId, DateTime start, DateTime end)
        {
            var orgList = organizationId > 0 ? new List<int>() { organizationId } : null;

            return await PersonActions
                    .DataOwnership(orgList, person)
                    .Where(a => a.Timestamp >= start && a.Timestamp <= end)
                    .OfType<PersonActionActivity>()
                    .Include(a => a.Person.Team)
                    .Include(a => a.Activity.Translations)
                    .ToListAsync();
        }

        public async Task<long> InsertOrUpdatePersonAsync(Person item)
        {
            return await PersonRepository.InsertOrUpdateAndGetIdAsync(item);
        }

        public async Task InsertPersonRoleAsync(PersonRole item)
        {
            //Check if association already exists
            var personRole = PersonRoles
                                .SingleOrDefault(pr => pr.PersonId == item.PersonId && pr.RoleId == item.RoleId);

            if(personRole == null)
                await PersonRoleRepository.InsertAsync(item);
        }

        public async Task<bool> CheckPersonIdAsync(long personId)
        {
            return await Persons.CountAsync(p => p.Id == personId) > 0;
        }

        public async Task<bool> CheckRoleIdAsync(long roleId)
        {
            return await Roles.CountAsync(r => r.Id == roleId) > 0;
        }

        public async Task<PersonAction> GetLastPersonActionAsync(long personId)
        {
            return await PersonActions
                        .Where(a => a.PersonId == personId)
                        .OrderBy(a => a.Timestamp)
                        .LastOrDefaultAsync();
        }

        public async Task<int> GetLastPersonActivityAsync(long personId)
        {
            return await PersonActions
                        .OfType<PersonActionActivity>()
                        .Where(a => a.PersonId == personId)
                        .OrderBy(a => a.Timestamp)
                        .Select(a => a.ActivityId)
                        .LastOrDefaultAsync();
        }

        public async Task DeletePersonRolesAsync(long personId)
        {
            await PersonRoleRepository.DeleteAsync(p => (p.PersonId == personId));
        }

        public async Task<bool> CanOrganizationBeDeletedAsync(int organizationId)
        {
            //No person associated to the organization
            if (await Persons.CountAsync(p => p.OrganizationId == organizationId) > 0)
                return false;

            //Organization must not have children
            if (await Organizations.CountAsync(o => o.ParentId.HasValue && o.ParentId.Value == organizationId) > 0)
                return false;

            return true;
        }

        public async Task<bool> CheckIfRoleExists(string role)
        {
            return await Roles.AnyAsync(r => r.Name == role);
        }

        public async Task<List<Person>> GetRegistrationTokensByOrganizationIdAsync(int orgId, long creatorId)
        {
            return await Persons
                            .Where(p => p.Id != creatorId)
                            .Where(p => p.OrganizationId.HasValue && p.OrganizationId.Value == orgId)
                            .ToListAsync();
        }

        public async Task<List<Person>> GetPersonsByOrganizationIdAsync(int orgId, bool excludeMe = true, long myId = 0)
        {
            return await Persons
                            .Where(p => p.OrganizationId.HasValue && p.OrganizationId.Value == orgId)
                            .WhereIf(excludeMe, p => p.Id != myId)
                            .ToListAsync();
        }

        public Person GetPersonByUsername(string username)
        {
            return Persons.SingleOrDefault(p => p.Username == username);
        }

        public Person GetPersonByEmail(string email)
        {
            return Persons.SingleOrDefault(p => p.Email == email);
        }

        public async Task<List<string>> GetPersonRoleNamesAsync(long personId)
        {
            return await PersonRoles
                .Where(pr => pr.PersonId == personId)
                .Select(pr => pr.Role.Name)
                .ToListAsync();
        }

        public async Task<List<PersonRole>> GetPersonRolesAsync(long personId)
        {
            return await PersonRoles
                .Where(pr => pr.PersonId == personId)
                .ToListAsync();
        }

        public async Task<List<Role>> GetRolesByName(List<string> roleNames)
        {
            return await Roles
                .Where(r =>roleNames.Contains(r.Name))
                .ToListAsync();
        }

        public async Task<Role> GetDefaultRole()
        {
            return await Roles.SingleOrDefaultAsync(r => r.Default);
        }

        public async Task<List<string>> GetTipsReadByPersonIdAsync(long personId)
        {
            return await PersonTips
                            .Where(pt => pt.PersonId == personId)
                            .Select(tp => tp.TipCode)
                            .ToListAsync();
        }

        public async Task<List<string>> GetQuizzesReadByPersonIdAsync(long personId)
        {
            return await PersonQuizzes
                            .Where(pt => pt.PersonId == personId)
                            .Select(tp => tp.QuizCode)
                            .ToListAsync();
        }

        public async Task<int> CreatePersonTipAsync(long personId, string tipCode)
        {
            if (PersonTips.Where(pt => pt.PersonId == personId && pt.TipCode == tipCode).Count() == 0)
            {
                var newItem = new PersonTip(personId, tipCode);
                return await PersonTipRepository.InsertAndGetIdAsync(newItem);
            }
            else
                return -1;
        }

        public async Task CreatePersonQuizAsync(long personId, string quizCode)
        {
            var newItem = new PersonQuiz(personId, quizCode);
            await PersonQuizRepository.InsertAsync(newItem);
        }

    }
}
