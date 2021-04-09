using Abp.SocialMedia.Dto;
using Abp.SocialMedia.Model;
using Ermes.Dto;
using Ermes.Interfaces;
using Ermes.Social.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.Social
{
    public interface ISocialAppService : IBackofficeApi
    {
        Task<Pagination> GetAnnotations(GetAnnotationsInput input);
        Task<Pagination> GetAuthors(SocialPaginationInput input);
        Task<SocialItemOutput<Author>> GetAuthorById(IdInput<long> input);
        Task<Pagination> GetEvents(GetEventsInput input);
        Task<SocialItemOutput<Event>> GetEventById(IdInput<int> input);
        Task<GetLabelsOutput> GetLabels(GetLabelsInput input);
        Task<SocialItemOutput<Label>> GetLabelById(IdInput<int> input);
        Task<Pagination> GetMedia(SocialPaginationInput input);
        Task<SocialItemOutput<Media>> GetMediaById(IdInput<long> input);
        Task<Pagination> GetTweets(SocialPaginationInput input);
        Task<SocialItemOutput<Tweet>> GetTweetById(IdInput<long> input);
        Task<StatisticsTweet> GetTweetStatistics(GetTweetStatisticsInput input);
        Task<StatisticsEvent> GetEventStatistics(GetEventStatisticsInput input);
    }
}
