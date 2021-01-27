using Abp.ErmesSocialNetCore.Api;
using Abp.ErmesSocialNetCore.Client;
using Abp.ErmesSocialNetCore.Model;
using Abp.ErmesSocialNetCore.Social.Configuration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.ErmesSocialNetCore.Social
{
    public class SocialManager : ISocialManager
    {
        private readonly ISocialConnectionProvider _connectionProvider;
        private const string ApiKeyHeaderName = "X-API-Key";
        private const string ApiSecretHeaderName = "X-API-Secret";
        public SocialManager(ISocialConnectionProvider connectionProvider)
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
            var lab = input.Labels == null || input.Labels.Count == 0 ? null : input.Labels;
            var lan = input.Language == null ? null : input.Language.ToString().ToLower();
            return await api.ApiV1AnnotationsGetAsync(input.Page, input.Limit, input.Start, input.End, lan, input.Informative, lab, sw, ne);
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
        public async Task<List<Label>> GetLabels(LabelQuery input)
        {
            var api = new LabelsApi();
            var task = input.Task == null ? null : input.Task.ToString().ToLower();
            return await api.ApiV1LabelsGetAsync(input.Operational, task);
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

    }
}
