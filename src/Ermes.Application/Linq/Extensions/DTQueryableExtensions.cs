using Abp.Linq.Expressions;
using Ermes.Communications;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Missions;
using Ermes.Notifications;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.ReportRequests;
using Ermes.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Ermes.Linq.Extensions
{
    public static class DTQueryableExtensions
    {
        public static IQueryable<T> DTFilterBy<T>(this IQueryable<T> query, IDTParameters parameters)
        {

            if (parameters.Search == null || string.IsNullOrEmpty(parameters.Search.Value))
                return query;

            query = ResolveLinqFilter(query, parameters.Search);

            return query;
        }

        public static IQueryable<T> DTOrderedBy<T>(this IQueryable<T> query, IDTParameters parameters)
        {
            if (parameters.Order == null || parameters.Order.Count == 0)
                return query;

            query = ResolveLinqOrder(query, parameters.Order);

            return query;

        }

        private static IQueryable<T> ResolveLinqFilter<T>(IQueryable<T> query, DTSearch search)
        {
            if (search.Value != null)
                search.Value = search.Value.Trim().ToLower();

            // TODO: Usare IoC per risolvere dinamicamente il Linq resolver
            if (typeof(T) == typeof(Person))
                return new PersonLinqFilterResolver().Resolve(query as IQueryable<Person>, search) as IQueryable<T>;
            if (typeof(T) == typeof(Mission))
                return new MissionLinqFilterResolver().Resolve(query as IQueryable<Mission>, search) as IQueryable<T>;
            if (typeof(T) == typeof(Report))
                return new ReportLinqFilterResolver().Resolve(query as IQueryable<Report>, search) as IQueryable<T>;
            if (typeof(T) == typeof(ReportRequest))
                return new ReportRequestLinqFilterResolver().Resolve(query as IQueryable<ReportRequest>, search) as IQueryable<T>;
            if (typeof(T) == typeof(Organization))
                return new OrganizationLinqFilterResolver().Resolve(query as IQueryable<Organization>, search) as IQueryable<T>;
            if (typeof(T) == typeof(Communication))
                return new CommunicationLinqFilterResolver().Resolve(query as IQueryable<Communication>, search) as IQueryable<T>;
            if (typeof(T) == typeof(Notification))
                return new NotificationLinqFilterResolver().Resolve(query as IQueryable<Notification>, search) as IQueryable<T>;

            return query;
        }

        private static IQueryable<T> ResolveLinqOrder<T>(IQueryable<T> query, List<DTOrder> order)
        {
            // TODO: Usare IoC per risolvere dinamicamente il Linq resolver
            if (typeof(T) == typeof(Mission))
                return new MissionLinqOrderResolver().Resolve(query as IQueryable<Mission>, order) as IQueryable<T>;
            if (typeof(T) == typeof(Person))
                return new PersonLinqOrderResolver().Resolve(query as IQueryable<Person>, order) as IQueryable<T>;
            if (typeof(T) == typeof(Report))
                return new ReportLinqOrderResolver().Resolve(query as IQueryable<Report>, order) as IQueryable<T>;
            if (typeof(T) == typeof(ReportRequest))
                return new ReportRequestLinqOrderResolver().Resolve(query as IQueryable<ReportRequest>, order) as IQueryable<T>;
            if (typeof(T) == typeof(Organization))
                return new OrganizationLinqOrderResolver().Resolve(query as IQueryable<Organization>, order) as IQueryable<T>;
            if (typeof(T) == typeof(Communication))
                return new CommunicationLinqOrderResolver().Resolve(query as IQueryable<Communication>, order) as IQueryable<T>;
            if (typeof(T) == typeof(Notification))
                return new NotificationLinqOrderResolver().Resolve(query as IQueryable<Notification>, order) as IQueryable<T>;

            return query;
        }

        #region Interface

        private interface ILinqFilterResolver<T>
        {
            IQueryable<T> Resolve(IQueryable<T> query, DTSearch search);
        }

        private interface ILinqOrderResolver<T>
        {
            IQueryable<T> Resolve(IQueryable<T> query, List<DTOrder> order);
        }

        #endregion

        #region Specific Linq resolver

        private class MissionLinqFilterResolver : ILinqFilterResolver<Mission>
        {
            public IQueryable<Mission> Resolve(IQueryable<Mission> query, DTSearch search)
            {
                var predicate = PredicateBuilder.New<Mission>(false);
                if (search.Regex) {
                    ///TBD
                    //predicate = predicate.Or(p => Regex.Match(p.Title, search.Value).Success);
                    //predicate = predicate.Or(p => Regex.Match(p.Description, search.Value).Success);
                }
                else
                {
                    predicate = predicate.Or(p => p.Title != null && p.Title.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.Description != null && p.Description.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);
            }
        }

        private class ReportLinqFilterResolver : ILinqFilterResolver<Report>
        {
            public IQueryable<Report> Resolve(IQueryable<Report> query, DTSearch search)
            {

                var predicate = PredicateBuilder.New<Report>(false);
                if (search.Regex)
                {
                    //TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Description != null && p.Description.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.Address != null && p.Address.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);
            }
        }

        private class ReportRequestLinqFilterResolver : ILinqFilterResolver<ReportRequest>
        {
            public IQueryable<ReportRequest> Resolve(IQueryable<ReportRequest> query, DTSearch search)
            {

                var predicate = PredicateBuilder.New<ReportRequest>(false);
                if (search.Regex)
                {
                    //TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Title != null && p.Title.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);
            }
        }

        private class OrganizationLinqFilterResolver : ILinqFilterResolver<Organization>
        {
            public IQueryable<Organization> Resolve(IQueryable<Organization> query, DTSearch search)
            {

                var predicate = PredicateBuilder.New<Organization>(false);
                if (search.Regex)
                {
                    ///TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Name != null && p.Name.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.ShortName != null && p.ShortName.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.Description != null && p.Description.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);

            }
        }

        private class PersonLinqFilterResolver : ILinqFilterResolver<Person>
        {
            public IQueryable<Person> Resolve(IQueryable<Person> query, DTSearch search)
            {

                var predicate = PredicateBuilder.New<Person>(false);
                if (search.Regex)
                {
                    ///TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Username != null && p.Username.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.FusionAuthUserGuid != null && p.FusionAuthUserGuid.ToString().ToLower().Contains(search.Value));
                }

                return query.Where(predicate);

            }
        }

        private class CommunicationLinqFilterResolver : ILinqFilterResolver<Communication>
        {
            public IQueryable<Communication> Resolve(IQueryable<Communication> query, DTSearch search)
            {

                var predicate = PredicateBuilder.New<Communication>(false);
                if (search.Regex)
                {
                    ///TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Message != null && p.Message.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);

            }
        }
        private class NotificationLinqFilterResolver : ILinqFilterResolver<Notification>
        {
            public IQueryable<Notification> Resolve(IQueryable<Notification> query, DTSearch search)
            {
                var predicate = PredicateBuilder.New<Notification>(false);
                if (search.Regex)
                {
                    ///TBD
                }
                else
                {
                    predicate = predicate.Or(p => p.Title.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.ChannelString.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.EntityString.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.Message.ToLower().Contains(search.Value));
                    predicate = predicate.Or(p => p.StatusString.ToLower().Contains(search.Value));
                }

                return query.Where(predicate);

            }
        }

        private class MissionLinqOrderResolver : ILinqOrderResolver<Mission>
        {
            public IQueryable<Mission> Resolve(IQueryable<Mission> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "title":
                            query = query.OrderBy(a => a.Title, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "description":
                            query = query.OrderBy(a => a.Description, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "notes":
                            query = query.OrderBy(a => a.Notes, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "currentstatus":
                            query = query.OrderBy(a => a.CurrentStatus, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "startdate":
                            query = query.OrderBy(a => a.Duration.LowerBound, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "enddate":
                            query = query.OrderBy(a => a.Duration.UpperBound, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderBy(a => a.Title);

                return query;
            }
        }

        private class ReportLinqOrderResolver : ILinqOrderResolver<Report>
        {
            public IQueryable<Report> Resolve(IQueryable<Report> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "timestamp":
                            query = query.OrderBy(a => a.Timestamp, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "description":
                            query = query.OrderBy(a => a.Description, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "notes":
                            query = query.OrderBy(a => a.Notes, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderByDescending(a => a.Timestamp);

                return query;
            }
        }

        private class ReportRequestLinqOrderResolver : ILinqOrderResolver<ReportRequest>
        {
            public IQueryable<ReportRequest> Resolve(IQueryable<ReportRequest> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "duration":
                            query = query.OrderBy(a => a.Duration.LowerBound, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "title":
                            query = query.OrderBy(a => a.Title, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderByDescending(a => a.Title);

                return query;
            }
        }

        private class OrganizationLinqOrderResolver : ILinqOrderResolver<Organization>
        {
            public IQueryable<Organization> Resolve(IQueryable<Organization> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "name":
                            query = query.OrderBy(a => a.Name, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "description":
                            query = query.OrderBy(a => a.Description, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "shortname":
                            query = query.OrderBy(a => a.ShortName, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderBy(a => a.Name);

                return query;
            }
        }

        private class PersonLinqOrderResolver : ILinqOrderResolver<Person>
        {
            public IQueryable<Person> Resolve(IQueryable<Person> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "surname":
                            query = query.OrderBy(a =>a.Username, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "fusionauthuserguid":
                            query = query.OrderBy(a => a.FusionAuthUserGuid, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "registrationtoken":
                            query = query.OrderBy(a => a.RegistrationToken, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderBy(a => a.Username);

                return query;
            }
        }

        private class CommunicationLinqOrderResolver : ILinqOrderResolver<Communication>
        {
            public IQueryable<Communication> Resolve(IQueryable<Communication> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "message":
                            query = query.OrderBy(a => a.Message, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "duration":
                            query = query.OrderBy(a => a.Duration.LowerBound, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderBy(a => a.Duration.LowerBound);

                return query;
            }
        }

        private class NotificationLinqOrderResolver : ILinqOrderResolver<Notification>
        {
            public IQueryable<Notification> Resolve(IQueryable<Notification> query, List<DTOrder> order)
            {
                bool firstOrderClause = true;

                foreach (var item in order)
                {
                    ListSortDirection direction = item.Dir == DTOrderDir.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    switch (item.Column.ToLower())
                    {
                        case "title":
                            query = query.OrderBy(a => a.Title, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "message":
                            query = query.OrderBy(a => a.Message, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "status":
                            query = query.OrderBy(a => a.Status, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "channel":
                            query = query.OrderBy(a => a.Channel, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        case "entity":
                            query = query.OrderBy(a => a.Entity, direction, firstOrderClause);
                            firstOrderClause = false;
                            break;
                        default:
                            break;
                    }
                }

                // At least one order criteria is needed
                if (firstOrderClause)
                    query = query.OrderBy(a => a.Timestamp);

                return query;
            }
        }
        #endregion

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source,
                Expression<Func<TSource, TKey>> keySelector, ListSortDirection sortOrder, bool firstOrderClause)
        {
            if (sortOrder == ListSortDirection.Ascending)
                return firstOrderClause ? source.OrderBy(keySelector) : ((IOrderedQueryable<TSource>)source).ThenBy(keySelector);
            else
                return firstOrderClause ? source.OrderByDescending(keySelector) : ((IOrderedQueryable<TSource>)source).ThenByDescending(keySelector);
        }
    }

    public class DTQueryableOrder<T>
    {
        public Expression<Func<T, dynamic>> OrderBy { get; set; }
        public ListSortDirection SortOrder { get; set; }
        public bool FirstOrderClause { get; set; }
    }
}
