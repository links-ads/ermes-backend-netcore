/*
 * Event Detection Module - Public API
 *
 *  ### Public endpoints for the Social Media Analysis module (SMA). 
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Abp.SocialMedia.Client;
using Abp.SocialMedia.Model;

namespace Abp.SocialMedia.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITweetsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Gets a paged list of tweets
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <returns>Pagination</returns>
        Pagination ApiV1TweetsGet(int? page = default(int?), int? limit = default(int?));

        /// <summary>
        /// Gets a paged list of tweets
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        ApiResponse<Pagination> ApiV1TweetsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?));
        /// <summary>
        /// Gets a single tweet
        /// </summary>
        /// <remarks>
        /// Retrieves a single tweet corresponding to the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <returns>Tweet</returns>
        Tweet ApiV1TweetsTweetIdGet(long tweetId);

        /// <summary>
        /// Gets a single tweet
        /// </summary>
        /// <remarks>
        /// Retrieves a single tweet corresponding to the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <returns>ApiResponse of Tweet</returns>
        ApiResponse<Tweet> ApiV1TweetsTweetIdGetWithHttpInfo(long tweetId);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITweetsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Gets a paged list of tweets
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        System.Threading.Tasks.Task<Pagination> ApiV1TweetsGetAsync(int? page = default(int?), int? limit = default(int?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Gets a paged list of tweets
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        System.Threading.Tasks.Task<ApiResponse<Pagination>> ApiV1TweetsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// Gets a single tweet
        /// </summary>
        /// <remarks>
        /// Retrieves a single tweet corresponding to the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Tweet</returns>
        System.Threading.Tasks.Task<Tweet> ApiV1TweetsTweetIdGetAsync(long tweetId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Gets a single tweet
        /// </summary>
        /// <remarks>
        /// Retrieves a single tweet corresponding to the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Tweet)</returns>
        System.Threading.Tasks.Task<ApiResponse<Tweet>> ApiV1TweetsTweetIdGetWithHttpInfoAsync(long tweetId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ITweetsApi : ITweetsApiSync, ITweetsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class TweetsApi : ITweetsApi
    {
        private Abp.SocialMedia.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TweetsApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TweetsApi(String basePath)
        {
            this.Configuration = Abp.SocialMedia.Client.Configuration.MergeConfigurations(
                Abp.SocialMedia.Client.GlobalConfiguration.Instance,
                new Abp.SocialMedia.Client.Configuration { BasePath = basePath }
            );
            this.Client = new Abp.SocialMedia.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Abp.SocialMedia.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = Abp.SocialMedia.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public TweetsApi(Abp.SocialMedia.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = Abp.SocialMedia.Client.Configuration.MergeConfigurations(
                Abp.SocialMedia.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new Abp.SocialMedia.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Abp.SocialMedia.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = Abp.SocialMedia.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public TweetsApi(Abp.SocialMedia.Client.ISynchronousClient client, Abp.SocialMedia.Client.IAsynchronousClient asyncClient, Abp.SocialMedia.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = Abp.SocialMedia.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public Abp.SocialMedia.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public Abp.SocialMedia.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Abp.SocialMedia.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Abp.SocialMedia.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets a paged list of tweets Retrieves a paged list of tweets.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <returns>Pagination</returns>
        public Pagination ApiV1TweetsGet(int? page = default(int?), int? limit = default(int?))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = ApiV1TweetsGetWithHttpInfo(page, limit);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a paged list of tweets Retrieves a paged list of tweets.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        public Abp.SocialMedia.Client.ApiResponse<Pagination> ApiV1TweetsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?))
        {
            Abp.SocialMedia.Client.RequestOptions localVarRequestOptions = new Abp.SocialMedia.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json"
            };

            var localVarContentType = Abp.SocialMedia.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Abp.SocialMedia.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (page != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "page", page));
            }
            if (limit != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "limit", limit));
            }

            // authentication (api_key) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }
            // authentication (api_secret) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Secret")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Secret", this.Configuration.GetApiKeyWithPrefix("X-API-Secret"));
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<Pagination>("/api/v1/tweets", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1TweetsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a paged list of tweets Retrieves a paged list of tweets.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        public async System.Threading.Tasks.Task<Pagination> ApiV1TweetsGetAsync(int? page = default(int?), int? limit = default(int?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = await ApiV1TweetsGetWithHttpInfoAsync(page, limit, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a paged list of tweets Retrieves a paged list of tweets.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional)</param>
        /// <param name="limit">how many instances per page (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<Pagination>> ApiV1TweetsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            Abp.SocialMedia.Client.RequestOptions localVarRequestOptions = new Abp.SocialMedia.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json"
            };


            var localVarContentType = Abp.SocialMedia.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Abp.SocialMedia.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            if (page != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "page", page));
            }
            if (limit != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "limit", limit));
            }

            // authentication (api_key) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }
            // authentication (api_secret) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Secret")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Secret", this.Configuration.GetApiKeyWithPrefix("X-API-Secret"));
            }

            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.GetAsync<Pagination>("/api/v1/tweets", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1TweetsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a single tweet Retrieves a single tweet corresponding to the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <returns>Tweet</returns>
        public Tweet ApiV1TweetsTweetIdGet(long tweetId)
        {
            Abp.SocialMedia.Client.ApiResponse<Tweet> localVarResponse = ApiV1TweetsTweetIdGetWithHttpInfo(tweetId);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single tweet Retrieves a single tweet corresponding to the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <returns>ApiResponse of Tweet</returns>
        public Abp.SocialMedia.Client.ApiResponse<Tweet> ApiV1TweetsTweetIdGetWithHttpInfo(long tweetId)
        {
            Abp.SocialMedia.Client.RequestOptions localVarRequestOptions = new Abp.SocialMedia.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json"
            };

            var localVarContentType = Abp.SocialMedia.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Abp.SocialMedia.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("tweet_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(tweetId)); // path parameter

            // authentication (api_key) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }
            // authentication (api_secret) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Secret")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Secret", this.Configuration.GetApiKeyWithPrefix("X-API-Secret"));
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<Tweet>("/api/v1/tweets/{tweet_id}", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1TweetsTweetIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a single tweet Retrieves a single tweet corresponding to the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Tweet</returns>
        public async System.Threading.Tasks.Task<Tweet> ApiV1TweetsTweetIdGetAsync(long tweetId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<Tweet> localVarResponse = await ApiV1TweetsTweetIdGetWithHttpInfoAsync(tweetId, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single tweet Retrieves a single tweet corresponding to the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="tweetId">ID of the tweet</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Tweet)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<Tweet>> ApiV1TweetsTweetIdGetWithHttpInfoAsync(long tweetId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            Abp.SocialMedia.Client.RequestOptions localVarRequestOptions = new Abp.SocialMedia.Client.RequestOptions();

            String[] _contentTypes = new String[] {
            };

            // to determine the Accept header
            String[] _accepts = new String[] {
                "application/json"
            };


            var localVarContentType = Abp.SocialMedia.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null) localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);

            var localVarAccept = Abp.SocialMedia.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null) localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);

            localVarRequestOptions.PathParameters.Add("tweet_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(tweetId)); // path parameter

            // authentication (api_key) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }
            // authentication (api_secret) required
            if (!String.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Secret")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Secret", this.Configuration.GetApiKeyWithPrefix("X-API-Secret"));
            }

            // make the HTTP request

            var localVarResponse = await this.AsynchronousClient.GetAsync<Tweet>("/api/v1/tweets/{tweet_id}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1TweetsTweetIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
