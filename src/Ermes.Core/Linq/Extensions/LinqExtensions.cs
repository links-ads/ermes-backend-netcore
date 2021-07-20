using Abp.Linq.Extensions;
using Ermes.Communications;
using Ermes.Interfaces;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.ReportRequests;
using Ermes.Reports;
using Ermes.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Linq.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> ValidAtDate<T>(this IQueryable<T> query, DateTime startDate, DateTime endDate) where T : class, ITimeVariant
        {
            //TBD
            return query
                    .Where(a =>a.StartDate >= startDate && a.EndDate <= endDate);
        }

        #region Data Ownership
        public static IQueryable<T> DataOwnership<T>(this IQueryable<T> query, List<int> organizationIdList, IPersonBase person =null)
        {

            if (organizationIdList == null || organizationIdList.Count == 0)
                organizationIdList = null;

            query = ResolveDataOwnership(query, organizationIdList, person);

            return query;
        }

        private static IQueryable<T> ResolveDataOwnership<T>(IQueryable<T> query, List<int> organizationIdList, IPersonBase person)
        {
            if (typeof(T) == typeof(Person))
                return new PersonDataOwnershipResolver().Resolve(query as IQueryable<Person>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(Mission))
                return new MissionDataOwnershipResolver().Resolve(query as IQueryable<Mission>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(Report))
                return new ReportDataOwnershipResolver().Resolve(query as IQueryable<Report>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(ReportRequest))
                return new ReportRequestDataOwnershipResolver().Resolve(query as IQueryable<ReportRequest>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(Organization))
                return new OrganizationDataOwnershipResolver().Resolve(query as IQueryable<Organization>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(PersonAction))
                return new PersonActionDataOwnershipResolver().Resolve(query as IQueryable<PersonAction>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(Communication))
                return new CommunicationDataOwnershipResolver().Resolve(query as IQueryable<Communication>, organizationIdList, person) as IQueryable<T>;
            else if (typeof(T) == typeof(Team))
                return new TeamDataOwnershipResolver().Resolve(query as IQueryable<Team>, organizationIdList, person) as IQueryable<T>;

            return query;
        }

        #endregion

        #region Data Ownership Interfaces
        private interface IDataOwnershipResolver<T>
        {
            IQueryable<T> Resolve(IQueryable<T> query, List<int> organizationIdList, IPersonBase person);
        }
        #endregion

        #region Data Ownership classes
        private class PersonDataOwnershipResolver : IDataOwnershipResolver<Person>
        {
            public IQueryable<Person> Resolve(IQueryable<Person> query, List<int> organizationIdList, IPersonBase person)
            {
                if(organizationIdList != null)
                    query = query
                            .Where(p => p.OrganizationId.HasValue && organizationIdList.Contains(p.OrganizationId.Value));

                return query;
            }
        }

        private class MissionDataOwnershipResolver : IDataOwnershipResolver<Mission>
        {
            public IQueryable<Mission> Resolve(IQueryable<Mission> query, List<int> organizationIdList, IPersonBase person)
            {
                return organizationIdList == null
                            ? query
                            //TODO: implement ADMIN visibility by using permission                            
                            : 
                            query
                            .Where(m => organizationIdList.Contains(m.OrganizationId) || (m.Organization.ParentId.HasValue && organizationIdList.Contains(m.Organization.ParentId.Value)));
            }
        }

        private class ReportDataOwnershipResolver : IDataOwnershipResolver<Report>
        {
            public IQueryable<Report> Resolve(IQueryable<Report> query, List<int> organizationIdList, IPersonBase person)
            {
                return organizationIdList == null
                    ? query
                            //TODO: implement ADMIN visibility by using permission
                            .Where(r => !r.Creator.OrganizationId.HasValue)
                    : query
                    .Where(r =>
                            //Organization visibility
                            (r.Creator.OrganizationId.HasValue && organizationIdList.Contains(r.Creator.OrganizationId.Value)) ||
                            //Organization hierarchy
                            (r.Creator.Organization.ParentId.HasValue && organizationIdList.Contains(r.Creator.Organization.ParentId.Value)) ||
                            //User contents
                            !r.Creator.OrganizationId.HasValue
                        );
            }
        }

        private class ReportRequestDataOwnershipResolver : IDataOwnershipResolver<ReportRequest>
        {
            public IQueryable<ReportRequest> Resolve(IQueryable<ReportRequest> query, List<int> organizationIdList, IPersonBase person)
            {
                return organizationIdList == null
                    ? query
                            .Where(r => !r.Creator.OrganizationId.HasValue)
                    : query
                    .Where(r =>
                                //Organization visibility
                                (r.Creator.OrganizationId.HasValue && organizationIdList.Contains(r.Creator.OrganizationId.Value)) ||
                                //User contents
                                !r.Creator.OrganizationId.HasValue
                            );
            }
        }

        private class OrganizationDataOwnershipResolver : IDataOwnershipResolver<Organization>
        {
            public IQueryable<Organization> Resolve(IQueryable<Organization> query, List<int> organizationIdList, IPersonBase person)
            {
                if(organizationIdList != null)
                    query = query
                            .Where(o => organizationIdList.Contains(o.Id) || (o.ParentId.HasValue && organizationIdList.Contains(o.ParentId.Value)));

                return query;

            }
        }

        private class PersonActionDataOwnershipResolver : IDataOwnershipResolver<PersonAction>
        {
            public IQueryable<PersonAction> Resolve(IQueryable<PersonAction> query, List<int> organizationIdList, IPersonBase person)
            {
                return query
                    .Where(pa => pa.Person.OrganizationId.HasValue && organizationIdList.Contains(pa.Person.OrganizationId.Value))
                    .WhereIf(person != null, pa => pa.PersonId == person.Id);
            }
        }

        private class CommunicationDataOwnershipResolver : IDataOwnershipResolver<Communication>
        {
            public IQueryable<Communication> Resolve(IQueryable<Communication> query, List<int> organizationIdList, IPersonBase person)
            {
                return organizationIdList == null
                        ? query
                        : query
                        .Where(r =>
                            //Organization visibility
                            (r.Creator.OrganizationId.HasValue && organizationIdList.Contains(r.Creator.OrganizationId.Value)) ||
                            //Organization hierarchy
                            (r.Creator.Organization.ParentId.HasValue && organizationIdList.Contains(r.Creator.Organization.ParentId.Value))
                        );
            }
        }

        private class TeamDataOwnershipResolver : IDataOwnershipResolver<Team>
        {
            public IQueryable<Team> Resolve(IQueryable<Team> query, List<int> organizationIdList, IPersonBase person)
            {
                if(organizationIdList != null)
                query =  query
                        .Where(t => organizationIdList.Contains(t.OrganizationId) || (t.Organization.ParentId.HasValue && organizationIdList.Contains(t.Organization.ParentId.Value)));

                return query;
            }
        }
        #endregion


    }
}
