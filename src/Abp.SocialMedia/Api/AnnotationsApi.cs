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
    public interface IAnnotationsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <returns>Pagination</returns>
        Pagination ApiV1AnnotationsGet(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>));

        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        ApiResponse<Pagination> ApiV1AnnotationsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>));
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IAnnotationsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        System.Threading.Tasks.Task<Pagination> ApiV1AnnotationsGetAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors
        /// </summary>
        /// <remarks>
        /// Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        System.Threading.Tasks.Task<ApiResponse<Pagination>> ApiV1AnnotationsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IAnnotationsApi : IAnnotationsApiSync, IAnnotationsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class AnnotationsApi : IAnnotationsApi
    {
        private Abp.SocialMedia.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AnnotationsApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AnnotationsApi(String basePath)
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
        /// Initializes a new instance of the <see cref="AnnotationsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public AnnotationsApi(Abp.SocialMedia.Client.Configuration configuration)
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
        /// Initializes a new instance of the <see cref="AnnotationsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public AnnotationsApi(Abp.SocialMedia.Client.ISynchronousClient client, Abp.SocialMedia.Client.IAsynchronousClient asyncClient, Abp.SocialMedia.Client.IReadableConfiguration configuration)
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
        /// Retrieves a paged list of annotated tweets and associated media and authors Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <returns>Pagination</returns>
        public Pagination ApiV1AnnotationsGet(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = ApiV1AnnotationsGetWithHttpInfo(page, limit, start, end, languages, informative, hazards, infotypes, southWest, northEast, entityLabelId, entityValue);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <returns>ApiResponse of Pagination</returns>
        public Abp.SocialMedia.Client.ApiResponse<Pagination> ApiV1AnnotationsGetWithHttpInfo(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>))
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
            if (languages != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "languages", languages));
            }
            if (informative != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "informative", informative));
            }
            if (hazards != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "hazards", hazards));
            }
            if (infotypes != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "infotypes", infotypes));
            }
            if (southWest != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "south_west", southWest));
            }
            if (northEast != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "north_east", northEast));
            }
            if (entityLabelId != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "entity_label_id", entityLabelId));
            }
            if (entityValue != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "entity_value", entityValue));
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
            var localVarResponse = this.Client.Get<Pagination>("/api/v1/annotations", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1AnnotationsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Pagination</returns>
        public async System.Threading.Tasks.Task<Pagination> ApiV1AnnotationsGetAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<Pagination> localVarResponse = await ApiV1AnnotationsGetWithHttpInfoAsync(page, limit, start, end, languages, informative, hazards, infotypes, southWest, northEast, entityLabelId, entityValue, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Retrieves a paged list of annotated tweets and associated media and authors Retrieves a paged list of tweets, with their respective author, media and annotations (classifications, entities).
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="page">which page to retrieve (optional, default to 1)</param>
        /// <param name="limit">how many instances per page (optional, default to 100)</param>
        /// <param name="start">Date and time lower bound, defaults to _end - 24 hours_ (UTC) (optional)</param>
        /// <param name="end">Date and time upper bound, defaults to _now_ (UTC) (optional)</param>
        /// <param name="languages">List of tweet languages in BCP47 format (optional)</param>
        /// <param name="informative">Retrieve only informative (or not informative) tweets (optional)</param>
        /// <param name="hazards">Retrieve only a specific set of comma-separated hazard types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;hazard_type. (optional)</param>
        /// <param name="infotypes">Retrieve only a specific set of comma-separated information types. Refer to the \&quot;id\&quot; field in /api/v1/labels for the complete list, filtering by task&#x3D;information_type. (optional)</param>
        /// <param name="southWest">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="northEast">bottom-left corner of the bounding box for a spatial query, in (lon, lat) format. (optional)</param>
        /// <param name="entityLabelId">List of entity label id in order to recover just some entity types. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="entityValue">List of string representing entity specific values to recover. To specify more than one, separate them with &#39;,&#39; (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Pagination)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<Pagination>> ApiV1AnnotationsGetWithHttpInfoAsync(int? page = default(int?), int? limit = default(int?), DateTime? start = default(DateTime?), DateTime? end = default(DateTime?), List<string> languages = default(List<string>), bool? informative = default(bool?), List<int> hazards = default(List<int>), List<int> infotypes = default(List<int>), List<float> southWest = default(List<float>), List<float> northEast = default(List<float>), List<decimal> entityLabelId = default(List<decimal>), List<string> entityValue = default(List<string>), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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
            if (languages != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "languages", languages));
            }
            if (informative != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "informative", informative));
            }
            if (hazards != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "hazards", hazards));
            }
            if (infotypes != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "infotypes", infotypes));
            }
            if (southWest != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "south_west", southWest));
            }
            if (northEast != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "north_east", northEast));
            }
            if (entityLabelId != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "entity_label_id", entityLabelId));
            }
            if (entityValue != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("csv", "entity_value", entityValue));
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

            var localVarResponse = await this.AsynchronousClient.GetAsync<Pagination>("/api/v1/annotations", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1AnnotationsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
