using Abp.SocialMedia.Api;
using Abp.SocialMedia.Client;
using Abp.SocialMedia.Configuration;
using Abp.SocialMedia.Dto;
using Abp.SocialMedia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.SocialMedia
{
    public class SocialMediaManager : ISocialMediaManager
    {
        private readonly ISocialMediaConnectionProvider _connectionProvider;
        private const string ApiKeyHeaderName = "X-API-Key";
        private const string ApiSecretHeaderName = "X-API-Secret";
        public SocialMediaManager(ISocialMediaConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            Configure();
        }

        private void Configure()
        {
            var apiKeyDict = new Dictionary<string, string>
            {
                { ApiKeyHeaderName, _connectionProvider.GetApiKey() },
                { ApiSecretHeaderName, _connectionProvider.GetApiSecret() }
            };
            GlobalConfiguration.Instance = new GlobalConfiguration(new Dictionary<string, string>(), apiKeyDict, new Dictionary<string, string>(), _connectionProvider.GetBasePath());
        }

        #region Annotations
        public async Task<Pagination> GetAnnotations(PagedGenericQuery input)
        {
            var api = new AnnotationsApi();
            List<string> lan;
            (input, lan) = SetFilterValues(input);
            return await api.ApiV1AnnotationsGetAsync(input.Page, input.Limit, input.Start, input.End, lan, input.Informative, input.Hazards, input.Infotypes, input.SouthWest, input.NorthEast);
        }
        #endregion

        #region Authors
        public async Task<Pagination> GetAuthors(int? page = null, int? limit = null)
        {
            var api = new AuthorsApi();
            return await api.ApiV1AuthorsGetAsync(page, limit);
        }

        public async Task<Author> GetAuthorById(long authorId)
        {
            var api = new AuthorsApi();
            return await api.ApiV1AuthorsAuthorIdGetAsync(authorId);
        }
        #endregion

        #region Labels
        public async Task<List<Label>> GetLabels(bool? operational, string task)
        {
            var api = new LabelsApi();
            return await api.ApiV1LabelsGetAsync(operational, task);
        }

        public async Task<Label> GetLabelById(int labelId)
        {
            var api = new LabelsApi();
            return await api.ApiV1LabelsLabelIdGetAsync(labelId);
        }
        #endregion

        #region Media
        public async Task<Pagination> GetMedia(int? page = null, int? limit = null)
        {
            var api = new MediaApi();
            return await api.ApiV1MediaGetAsync(page, limit);
        }

        public async Task<Media> GetMediaById(long mediaId)
        {
            var api = new MediaApi();
            return await api.ApiV1MediaMediaIdGetAsync(mediaId);
        }
        #endregion

        #region Tweets
        public async Task<Pagination> GetTweets(int? page = null, int? limit = null)
        {
            var api = new TweetsApi();
            return await api.ApiV1TweetsGetAsync(page, limit);
        }

        public async Task<Tweet> GetTweetById(long tweetId)
        {
            var api = new TweetsApi();
            return await api.ApiV1TweetsTweetIdGetAsync(tweetId);
        }
        #endregion

        #region Events
        public async Task<Pagination> GetEvents(GetEventsQuery input)
        {
            var api = new EventsApi();
            List<string> lan;
            (input, lan) = SetFilterValues(input);           
            return await api.ApiV1EventsGetAsync(input.Page, input.Limit, input.Start, input.End, lan, input.Hazards, input.Infotypes, input.SouthWest, input.NorthEast);
        }

        public async Task<EventDetails> GetEventById(int eventId)
        {
            var api = new EventsApi();
            return await api.ApiV1EventsEventIdGetAsync(eventId);
        }
        #endregion

        #region Statistics

        public async Task<StatisticsTweet> GetTweetStatistics(GenericQuery input)
        {
            var api = new StatisticsApi();
            List<string> lan;
            (input, lan) = SetFilterValues(input);
            return await api.ApiV1StatsTweetsGetAsync(input.Start, input.End, lan, input.Informative, input.Hazards, input.Infotypes, input.SouthWest, input.NorthEast);
        }

        public async Task<StatisticsEvent> GetEventStatistics(EventStatsQuery input)
        {
            var api = new StatisticsApi();
            List<string> lan;
            (input, lan) = SetFilterValues(input);
            return await api.ApiV1StatsEventsGetAsync(input.Start, input.End, lan, input.Hazards, input.Infotypes, input.SouthWest, input.NorthEast);
        }
        #endregion

        #region Private
        private Tuple<PagedGenericQuery,List<string>> SetFilterValues(PagedGenericQuery input)
        {
            input.SouthWest = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            input.NorthEast = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            input.Hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            input.Infotypes = input.Infotypes == null || input.Infotypes.Count == 0 ? null : input.Infotypes;
            var lan = input.Languages == null || input.Languages.Count == 0 ? null : input.Languages.Select(l => l.ToString()).ToList();
            return new Tuple<PagedGenericQuery, List<string>>(input, lan);
        }

        private Tuple<GenericQuery, List<string>> SetFilterValues(GenericQuery input)
        {
            input.SouthWest = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            input.NorthEast = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            input.Hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            input.Infotypes = input.Infotypes == null || input.Infotypes.Count == 0 ? null : input.Infotypes;
            var lan = input.Languages == null || input.Languages.Count == 0 ? null : input.Languages.Select(l => l.ToString()).ToList();
            return new Tuple<GenericQuery, List<string>>(input, lan);
        }

        private Tuple<GetEventsQuery, List<string>> SetFilterValues(GetEventsQuery input)
        {
            input.SouthWest = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            input.NorthEast = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            input.Hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            input.Infotypes = input.Infotypes == null || input.Infotypes.Count == 0 ? null : input.Infotypes;
            var lan = input.Languages == null || input.Languages.Count == 0 ? null : input.Languages.Select(l => l.ToString()).ToList();
            return new Tuple<GetEventsQuery, List<string>>(input, lan);
        }

        private Tuple<EventStatsQuery, List<string>> SetFilterValues(EventStatsQuery input)
        {
            input.SouthWest = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            input.NorthEast = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            input.Hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            input.Infotypes = input.Infotypes == null || input.Infotypes.Count == 0 ? null : input.Infotypes;
            var lan = input.Languages == null || input.Languages.Count == 0 ? null : input.Languages.Select(l => l.ToString()).ToList();
            return new Tuple<EventStatsQuery, List<string>>(input, lan);
        }
        #endregion
    }
}
