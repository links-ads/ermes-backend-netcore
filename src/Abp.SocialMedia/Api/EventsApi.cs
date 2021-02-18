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
    public interface IEventsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Gets a single event (with tweet id associated)
        /// </summary>
        /// <remarks>
        /// Retrieves a single event with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <returns>EventDetails</returns>
        EventDetails ApiV1EventsEventIdGet(int eventId);

        /// <summary>
        /// Gets a single event (with tweet id associated)
        /// </summary>
        /// <remarks>
        /// Retrieves a single event with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <returns>ApiResponse of EventDetails</returns>
        ApiResponse<EventDetails> ApiV1EventsEventIdGetWithHttpInfo(int eventId);
        /// <summary>
        /// Retrieves a paged list of events
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of events
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <returns>Pagination</returns>
        Pagination ApiV1EventsGet(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>));

        /// <summary>
        /// Retrieves a paged list of events
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of events
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        ApiResponse<Pagination> ApiV1EventsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>));
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IEventsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Gets a single event (with tweet id associated)
        /// </summary>
        /// <remarks>
        /// Retrieves a single event with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of EventDetails</returns>
        System.Threading.Tasks.Task<EventDetails> ApiV1EventsEventIdGetAsync(int eventId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Gets a single event (with tweet id associated)
        /// </summary>
        /// <remarks>
        /// Retrieves a single event with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (EventDetails)</returns>
        System.Threading.Tasks.Task<ApiResponse<EventDetails>> ApiV1EventsEventIdGetWithHttpInfoAsync(int eventId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// Retrieves a paged list of events
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of events
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        System.Threading.Tasks.Task<Pagination> ApiV1EventsGetAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Retrieves a paged list of events
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of events
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        System.Threading.Tasks.Task<ApiResponse<Pagination>> ApiV1EventsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IEventsApi : IEventsApiSync, IEventsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class EventsApi : IEventsApi
    {
        private Abp.SocialMedia.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public EventsApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public EventsApi(String basePath)
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
        /// Initializes a new instance of the <see cref="EventsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public EventsApi(Abp.SocialMedia.Client.Configuration configuration)
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
        /// Initializes a new instance of the <see cref="EventsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public EventsApi(Abp.SocialMedia.Client.ISynchronousClient client, Abp.SocialMedia.Client.IAsynchronousClient asyncClient, Abp.SocialMedia.Client.IReadableConfiguration configuration)
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
        /// Gets a single event (with tweet id associated) Retrieves a single event with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <returns>EventDetails</returns>
        public EventDetails ApiV1EventsEventIdGet(int eventId)
        {
            Abp.SocialMedia.Client.ApiResponse<EventDetails> localVarResponse = ApiV1EventsEventIdGetWithHttpInfo(eventId);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single event (with tweet id associated) Retrieves a single event with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <returns>ApiResponse of EventDetails</returns>
        public Abp.SocialMedia.Client.ApiResponse<EventDetails> ApiV1EventsEventIdGetWithHttpInfo(int eventId)
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

            localVarRequestOptions.PathParameters.Add("event_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(eventId)); // path parameter

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
            var localVarResponse = this.Client.Get<EventDetails>("/api/v1/events/{event_id}", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1EventsEventIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a single event (with tweet id associated) Retrieves a single event with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of EventDetails</returns>
        public async System.Threading.Tasks.Task<EventDetails> ApiV1EventsEventIdGetAsync(int eventId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<EventDetails> localVarResponse = await ApiV1EventsEventIdGetWithHttpInfoAsync(eventId, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single event (with tweet id associated) Retrieves a single event with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventId">ID of the event</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (EventDetails)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<EventDetails>> ApiV1EventsEventIdGetWithHttpInfoAsync(int eventId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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

            localVarRequestOptions.PathParameters.Add("event_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(eventId)); // path parameter

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

            var localVarResponse = await this.AsynchronousClient.GetAsync<EventDetails>("/api/v1/events/{event_id}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1EventsEventIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Retrieves a paged list of events Retrieves a paged list of events
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <returns>Pagination</returns>
        public Pagination ApiV1EventsGet(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = ApiV1EventsGetWithHttpInfo(page, limit, start, end, verified, hazards, southWest, northEast);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Retrieves a paged list of events Retrieves a paged list of events
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        public Abp.SocialMedia.Client.ApiResponse<Pagination> ApiV1EventsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>))
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
            if (start != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "start", start));
            }
            if (end != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "end", end));
            }
            if (verified != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "verified", verified));
            }
            if (hazards != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("multi", "hazards", hazards));
            }
            if (southWest != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "south_west", southWest));
            }
            if (northEast != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "north_east", northEast));
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
            var localVarResponse = this.Client.Get<Pagination>("/api/v1/events", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1EventsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Retrieves a paged list of events Retrieves a paged list of events
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        public async System.Threading.Tasks.Task<Pagination> ApiV1EventsGetAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = await ApiV1EventsGetWithHttpInfoAsync(page, limit, start, end, verified, hazards, southWest, northEast, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Retrieves a paged list of events Retrieves a paged list of events
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - one hour_ (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (optional)</param>
        /// <param name="verified">Filter for verified events (optional)</param>
        /// <param name="hazards">Filter for a specific list of hazard types ids, specify more ids separating by comma (e.g 12, 13) (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<Pagination>> ApiV1EventsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), bool? verified = default(bool?), List<int> hazards = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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
            if (start != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "start", start));
            }
            if (end != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "end", end));
            }
            if (verified != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "verified", verified));
            }
            if (hazards != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("multi", "hazards", hazards));
            }
            if (southWest != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "south_west", southWest));
            }
            if (northEast != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "north_east", northEast));
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

            var localVarResponse = await this.AsynchronousClient.GetAsync<Pagination>("/api/v1/events", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1EventsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}