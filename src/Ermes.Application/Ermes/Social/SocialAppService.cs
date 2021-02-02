using Abp.ErmesSocialNetCore.Model;
using Abp.ErmesSocialNetCore.Social;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Dto;
using Ermes.Social.Dto;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Social
{
    [ErmesAuthorize]
    public class SocialAppService : ErmesAppServiceBase, ISocialAppService
    {
        private readonly SocialManager _socialManager;
        public SocialAppService(SocialManager socialManager)
        {
            _socialManager = socialManager;
        }

        #region Annotations
        [OpenApiOperation("Get Annotations",
            @"
                Retrieves a paged list of annotated tweets and associated media and authors
                Input: use the following properties to filter result list:
                    -Page: which page to retrieve (optional)
                    -Limit: how many instances per page (optional)
                    -Start: date and time lower bound (optional) (default: today - 2d) (max time window: 4 days)
                    -End: date and time upper bound (optional) (default: now) (max time window: 4 days)
                    -Language: language of the tweet (optional)
                    -Informative: retrieve only informative (or not informative) tweets (optional)
                    -Labels: retrieve only a specific set of comma-separated labels. Refer to the 'name' field in /app/Social/GetLabels for the complete list. The labels allow to filter for hazard type and information type. (optional)
                    -SouthWest: bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with NorthEast property)
                    -NorthEast: top-right corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with SouthWest property)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        public virtual async Task<Pagination> GetAnnotations(GetAnnotationsInput input)
        {
            try
            {
                AnnotationQuery annQuery = ObjectMapper.Map<AnnotationQuery>(input.Filters);
                return await _socialManager.GetAnnotations(annQuery);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        #endregion

        #region Authors
        [OpenApiOperation("Get auhtors",
            @"
                Gets a paged list of authors
                Input: use the following properties to filter result list:
                    -Page: which page to retrieve (optional)
                    -Limit: how many instances per page (optional)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        public virtual async Task<Pagination> GetAuthors(SocialPaginationInput input)
        {
            try
            {
                return await _socialManager.GetAuthors(input.Page, input.Limit);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [OpenApiOperation("Get auhtor by Id",
            @"
                Gets a single author
                Input: the id of the author
                Output: SocialItemOutput Dto with Author item
            "
        )]
        public virtual async Task<SocialItemOutput<Author>> GetAuthorById(IdInput<long> input)
        {
            try
            {
                return new SocialItemOutput<Author>()
                {
                    Item = await _socialManager.GetAuthorById(input.Id)
                };
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion

        #region Labels
        [OpenApiOperation("Get labels",
            @"
                Gets a paged list of labels entities.
                Input: use the following properties to filter result list:
                    -Task: Classification type (optional)
                    -Operational: retrieve only operational labels (optional)
                Exception: thrown when fails to make API call
                Output: SocialPaginationOutput Dto with list of Items
            "
        )]
        public virtual async Task<GetLabelsOutput> GetLabels(GetLabelsInput input)
        {
            try
            {
                LabelQuery labelQuery = ObjectMapper.Map<LabelQuery>(input.Filters);
                return new GetLabelsOutput()
                {
                    Labels = await _socialManager.GetLabels(labelQuery)
                };
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        [OpenApiOperation("Get label by Id",
            @"
                Gets a single label entity.
                Input: the id of the label
                Output: SocialItemOutput Dto with Label item
            "
        )]
        public virtual async Task<SocialItemOutput<Label>> GetLabelById(IdInput<int> input)
        {
            try
            {
                return new SocialItemOutput<Label>()
                {
                    Item = await _socialManager.GetLabelById(input.Id)
                };
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion

        #region Media
        [OpenApiOperation("Get media",
            @"
                Gets a paged list of media entities.
                Input: use the following properties to filter result list:
                    -Page: which page to retrieve (optional)
                    -Limit: how many instances per page (optional)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        public virtual async Task<Pagination> GetMedia(SocialPaginationInput input)
        {
            try
            {
                return await _socialManager.GetMedia(input.Page, input.Limit);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [OpenApiOperation("Get media by Id",
            @"
                Gets a single media entity.
                Input: the id of the media
                Output: SocialItemOutput Dto with Media item
            "
        )]
        public virtual async Task<SocialItemOutput<Media>> GetMediaById(IdInput<long> input)
        {
            try
            {
                return new SocialItemOutput<Media>()
                {
                    Item = await _socialManager.GetMediaById(input.Id)
                };
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion

        #region Tweets
        [OpenApiOperation("Get tweets",
            @"
                Gets a paged list of tweets.
                Input: use the following properties to filter result list:
                    -Page: which page to retrieve (optional)
                    -Limit: how many instances per page (optional)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        public virtual async Task<Pagination> GetTweets(SocialPaginationInput input)
        {
            try
            {
                return await _socialManager.GetTweets(input.Page, input.Limit);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [OpenApiOperation("Get tweet by Id",
            @"
                Gets a single tweet.
                Input: the id of the tweet
                Output: SocialItemOutput Dto with Tweet item
            "
        )]
        public virtual async Task<SocialItemOutput<Tweet>> GetTweetById(IdInput<long> input)
        {
            try
            {
                return new SocialItemOutput<Tweet>()
                {
                    Item = await _socialManager.GetTweetById(input.Id)
                };
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion
    }
}
