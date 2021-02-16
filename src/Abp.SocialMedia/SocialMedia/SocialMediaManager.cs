using Abp.SocialMedia.Api;
using Abp.SocialMedia.Client;
using Abp.SocialMedia.Configuration;
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
        public async Task<Pagination> GetAnnotations(AnnotationQuery input)
        {
            var api = new AnnotationsApi();
            var sw = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            var ne = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            var hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            var infot = input.Infotypes == null || input.Infotypes.Count == 0 ? null : input.Infotypes;
            var lan = input.Languages == null || input.Languages.Count == 0 ? null : input.Languages.Select(l => l.ToString()).ToList();
            return await api.ApiV1AnnotationsGetAsync(input.Page, input.Limit, input.Start, input.End, lan, input.Informative, hazards, infot, sw, ne);
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
        public async Task<List<Label>> GetLabels(bool? operational, string task = null)
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
        public async Task<Pagination> GetEvents(EventQuery input)
        {
            var api = new EventsApi();
            var sw = input.SouthWest == null || input.SouthWest.Count == 0 ? null : input.SouthWest;
            var ne = input.NorthEast == null || input.NorthEast.Count == 0 ? null : input.NorthEast;
            var hazards = input.Hazards == null || input.Hazards.Count == 0 ? null : input.Hazards;
            return await api.ApiV1EventsGetAsync(input.Page, input.Limit, input.Start, input.End, input.Verified, hazards, sw, ne);
        }

        public async Task<EventDetails> GetEventById(int eventId)
        {
            var api = new EventsApi();
            return await api.ApiV1EventsEventIdGetAsync(eventId);
        }
        #endregion

        #region Statistics
        public async Task<Statistics> GetStatistics(DateTime? startDate, DateTime? endDate)
        {
            var api = new StatisticsApi();
            return await api.ApiV1StatsGetAsync(startDate, endDate);
        }
        #endregion
    }
}
