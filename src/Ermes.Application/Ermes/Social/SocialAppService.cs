using Abp.SocialMedia;
using Abp.SocialMedia.Dto;
using Abp.SocialMedia.Model;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Dto;
using Ermes.Enums;
using Ermes.Social.Dto;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Social
{
    [ErmesAuthorize]
    public class SocialAppService : ErmesAppServiceBase, ISocialAppService
    {
        private readonly SocialMediaManager _socialManager;
        public SocialAppService(SocialMediaManager socialManager)
        {
            _socialManager = socialManager;
        }

        //N.B.: there's the necessity to wrap class coming from the SDK with Input class
        //with a constructor that initialises Filters property,
        //otherwise this module throws an exception when an API is called

        #region Annotations
        [OpenApiOperation("Get Annotations",
            @"
                Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities)
                Input: use the following properties to filter result list:
                    - Page: which page to retrieve (optional)
                    - Limit: how many instances per page (optional)
                    - Start: date and time lower bound (optional) (default: end - 24h)
                    - End: date and time upper bound (optional) (default: now)
                    - Languages: list of languages in BCP47 format (optional)
                    - Informative: retrieve only informative (or not informative) tweets (optional)
                    - Hazards: retrieve only a specific set of comma-separated hazard types. Refer to the 'id' field in 'GetLabels' api for the complete list, filtering 
                      by task=hazard_type (optional)
                    - Infotypes : retrieve only a specific set of comma-separated information types. Refer to the 'id' field in 'GetLabels' api for the complete list, filtering 
                      by task=information_type (optional)
                    - SouthWest: bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with NorthEast property)
                    - NorthEast: top-right corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with SouthWest property)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        // PagedGenericQuery cannot be used directly, SMA throws exception
        public virtual async Task<Pagination> GetAnnotations(GetAnnotationsInput input)
        {
            try
            {
                var filters = ObjectMapper.Map<PagedGenericQuery>(input.Filters);
                return await _socialManager.GetAnnotations(filters);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

        }
        #endregion

        #region Authors
        [OpenApiOperation("Get authors",
            @"
                Gets a paged list of authors
                Input: use the following properties to filter result list:
                    - Page: which page to retrieve (optional)
                    - Limit: how many instances per page (optional)
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

        #region Events
        [OpenApiOperation("Get Events",
            @"
                Gets a list of events
                Input: use the following properties to filter result list:
                    - Page: which page to retrieve (optional)
                    - Limit: how many instances per page (optional)
                    - Start: date and time lower bound (optional) (default: end - 24h)
                    - End: date and time upper bound (optional) (default: now)
                    - Languages: list of languages in BCP47 format (optional)
                    - Hazards: retrieve only a specific set of comma-separated hazard types. Refer to the 'id' field in 'GetLabels' api for the complete list, filtering 
                      by task=hazard_type (optional)
                    - Infotypes : retrieve only a specific set of comma-separated information types. Refer to the 'id' field in 'GetLabels' api for the complete list, filtering 
                      by task=information_type (optional)
                    - SouthWest: bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with NorthEast property)
                    - NorthEast: top-right corner of the bounding box for a spatial query, in (lon, lat) format. (optional) (to be filled together with SouthWest property)
                Exception: thrown when fails to make API call
                Output: Pagination Dto with list of Items
            "
        )]
        public virtual async Task<Pagination> GetEvents(GetEventsInput input)
        {
            try
            {
                var filters = ObjectMapper.Map<GetEventsQuery>(input.Filters);
                return await _socialManager.GetEvents(filters);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        [OpenApiOperation("Get Event by Id",
            @"
                Gets a single event
                Input: the id of the event
                Output: SocialItemOutput Dto with Event item
            "
        )]
        public virtual async Task<SocialItemOutput<Event>> GetEventById(IdInput<int> input)
        {
            try
            {
                return new SocialItemOutput<Event>()
                {
                    Item = await _socialManager.GetEventById(input.Id)
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
                    - Task: Classification type (optional)
                    - Operational: retrieve only operational labels (optional)
                Exception: thrown when fails to make API call
                Output: SocialPaginationOutput Dto with list of Items
            "
        )]
        public virtual async Task<GetLabelsOutput> GetLabels(GetLabelsInput input)
        {
            try
            {
                //Cannot use LabelQuery in GetLabels function.
                //Wrong defifinition of Task property, there's no sync between enum member and enum type
                //Deserialization in SMM throws an exception
                return new GetLabelsOutput()
                {
                    Labels = await _socialManager.GetLabels(input.Filters.Operational, input.Filters.Task == SocialModuleTaskType.none ? null : input.Filters.Task.ToString())
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
                    - Page: which page to retrieve (optional)
                    - Limit: how many instances per page (optional)
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
                    - Page: which page to retrieve (optional)
                    - Limit: how many instances per page (optional)
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

        #region Statistics
        public virtual async Task<StatisticsTweet> GetTweetStatistics(GetTweetStatisticsInput input)
        {
            try
            {
                var filters = ObjectMapper.Map<GenericQuery>(input.Filters);
                return await _socialManager.GetTweetStatistics(filters);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public virtual async Task<StatisticsEvent> GetEventStatistics(GetEventStatisticsInput input)
        {
            try
            {
                var filters = ObjectMapper.Map<EventStatsQuery>(input.Filters);
                return await _socialManager.GetEventStatistics(filters);
            }
            catch (System.Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        #endregion
    }
}
