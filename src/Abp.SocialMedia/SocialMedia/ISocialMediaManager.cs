using Abp.SocialMedia.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.SocialMedia
{
    public interface ISocialMediaManager
    {
        Task<Pagination> GetAnnotations(AnnotationQuery input);
        Task<Pagination> GetAuthors(int? page = null, int? limit = null);
        Task<Author> GetAuthorById(long authorId);
        Task<List<Label>> GetLabels(bool? operational, string task = null);
        Task<Label> GetLabelById(int labelId);
        Task<Pagination> GetMedia(int? page = null, int? limit = null);
        Task<Media> GetMediaById(long mediaId);
        Task<Pagination> GetTweets(int? page = null, int? limit = null);
        Task<Tweet> GetTweetById(long tweetId);
        Task<EventDetails> GetEventById(int eventId);
        Task<Pagination> GetEvents(EventQuery input);
        Task<Statistics> GetStatistics(DateTime? startDate, DateTime? endDate);
    }
}
