﻿using Abp.SocialMedia.Dto;
using Abp.SocialMedia.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Abp.SocialMedia
{
    public interface ISocialMediaManager
    {
        Task<Pagination> GetAnnotations(PagedGenericQuery input);
        Task<Pagination> GetAuthors(int? page = null, int? limit = null);
        Task<Author> GetAuthorById(long authorId);
        Task<List<Label>> GetLabels(bool? operational, string task);
        Task<Label> GetLabelById(int labelId);
        Task<Pagination> GetMedia(int? page = null, int? limit = null);
        Task<Media> GetMediaById(long mediaId);
        Task<Pagination> GetTweets(int? page = null, int? limit = null);
        Task<Tweet> GetTweetById(long tweetId);
        Task<Pagination> GetEvents(GetEventsQuery input);
        Task<Event> GetEventById(int eventId);
        Task<StatisticsTweet> GetTweetStatistics(GenericQuery input);
        Task<StatisticsEvent> GetEventStatistics(EventStatsQuery input);
    }
}
