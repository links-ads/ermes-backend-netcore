using Polly.Retry;
using RestSharp;

namespace Abp.SocialMedia.Client
{
    /// <summary>
    /// Configuration class to set the polly retry policies to be applied to the requests.
    /// </summary>
    public class RetryConfiguration
    {
        /// <summary>
        /// Retry policy
        /// </summary>
        public static RetryPolicy<IRestResponse> RetryPolicy { get; set; }

        /// <summary>
        /// Async retry policy
        /// </summary>
        public static AsyncRetryPolicy<IRestResponse> AsyncRetryPolicy { get; set; }
    }
}