using Abp.ErmesSocialNetCore.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.ErmesSocialNetCore.Social
{
    public interface ISocialManager
    {
        Task<Pagination> GetAnnotations(AnnotationQuery input);
        Task<Pagination> GetAuthors(int? page = null, int? limit = null);
        Task<Author> GetAuthorById(long authorId);
        //Task<List<Label>> GetLabels(bool operational = false, string task = null);
        Task<Label> GetLabelById(int labelId);
        Task<Pagination> GetMedia(int? page = null, int? limit = null);
        Task<Media> GetMediaById(long mediaId);
        Task<Pagination> GetTweets(int? page = null, int? limit = null);
        Task<Tweet> GetTweetById(long tweetId);
    }
}
