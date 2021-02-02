using Abp.Application.Services;
using Abp.ErmesSocialNetCore.Model;
using Ermes.Dto;
using Ermes.Social.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Social
{
    public interface ISocialAppService : IApplicationService
    {
        Task<Pagination> GetAnnotations(GetAnnotationsInput input);
        Task<Pagination> GetAuthors(SocialPaginationInput input);
        Task<SocialItemOutput<Author>> GetAuthorById(IdInput<long> input);
        Task<GetLabelsOutput> GetLabels(GetLabelsInput input);
        Task<SocialItemOutput<Label>> GetLabelById(IdInput<int> input);
        Task<Pagination> GetMedia(SocialPaginationInput input);
        Task<SocialItemOutput<Media>> GetMediaById(IdInput<long> input);
        Task<Pagination> GetTweets(SocialPaginationInput input);
        Task<SocialItemOutput<Tweet>> GetTweetById(IdInput<long> input);
        
    }
}
