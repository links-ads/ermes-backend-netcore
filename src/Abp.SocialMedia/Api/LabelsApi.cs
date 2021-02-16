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
    public interface ILabelsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Gets a list of labels
        /// </summary>
        /// <remarks>
        /// Retrieves a simple list of label entities.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <returns>List&lt;Label&gt;</returns>
        List<Label> ApiV1LabelsGet(bool? operational = default(bool?), string task = default(string));

        /// <summary>
        /// Gets a list of labels
        /// </summary>
        /// <remarks>
        /// Retrieves a simple list of label entities.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <returns>ApiResponse of List&lt;Label&gt;</returns>
        ApiResponse<List<Label>> ApiV1LabelsGetWithHttpInfo(bool? operational = default(bool?), string task = default(string));
        /// <summary>
        /// Gets a single label
        /// </summary>
        /// <remarks>
        /// Retrieves a single label with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <returns>Label</returns>
        Label ApiV1LabelsLabelIdGet(int labelId);

        /// <summary>
        /// Gets a single label
        /// </summary>
        /// <remarks>
        /// Retrieves a single label with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <returns>ApiResponse of Label</returns>
        ApiResponse<Label> ApiV1LabelsLabelIdGetWithHttpInfo(int labelId);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ILabelsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Gets a list of labels
        /// </summary>
        /// <remarks>
        /// Retrieves a simple list of label entities.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of List&lt;Label&gt;</returns>
        System.Threading.Tasks.Task<List<Label>> ApiV1LabelsGetAsync(bool? operational = default(bool?), string task = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Gets a list of labels
        /// </summary>
        /// <remarks>
        /// Retrieves a simple list of label entities.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (List&lt;Label&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<Label>>> ApiV1LabelsGetWithHttpInfoAsync(bool? operational = default(bool?), string task = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// Gets a single label
        /// </summary>
        /// <remarks>
        /// Retrieves a single label with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Label</returns>
        System.Threading.Tasks.Task<Label> ApiV1LabelsLabelIdGetAsync(int labelId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Gets a single label
        /// </summary>
        /// <remarks>
        /// Retrieves a single label with the ID provided in the path.
        /// </remarks>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Label)</returns>
        System.Threading.Tasks.Task<ApiResponse<Label>> ApiV1LabelsLabelIdGetWithHttpInfoAsync(int labelId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface ILabelsApi : ILabelsApiSync, ILabelsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class LabelsApi : ILabelsApi
    {
        private Abp.SocialMedia.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public LabelsApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public LabelsApi(String basePath)
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
        /// Initializes a new instance of the <see cref="LabelsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public LabelsApi(Abp.SocialMedia.Client.Configuration configuration)
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
        /// Initializes a new instance of the <see cref="LabelsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public LabelsApi(Abp.SocialMedia.Client.ISynchronousClient client, Abp.SocialMedia.Client.IAsynchronousClient asyncClient, Abp.SocialMedia.Client.IReadableConfiguration configuration)
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
        /// Gets a list of labels Retrieves a simple list of label entities.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <returns>List&lt;Label&gt;</returns>
        public List<Label> ApiV1LabelsGet(bool? operational = default(bool?), string task = default(string))
        {
            Abp.SocialMedia.Client.ApiResponse<List<Label>> localVarResponse = ApiV1LabelsGetWithHttpInfo(operational, task);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a list of labels Retrieves a simple list of label entities.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <returns>ApiResponse of List&lt;Label&gt;</returns>
        public Abp.SocialMedia.Client.ApiResponse<List<Label>> ApiV1LabelsGetWithHttpInfo(bool? operational = default(bool?), string task = default(string))
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

            if (operational != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "operational", operational));
            }
            if (task != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "task", task));
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
            var localVarResponse = this.Client.Get<List<Label>>("/api/v1/labels", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1LabelsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a list of labels Retrieves a simple list of label entities.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of List&lt;Label&gt;</returns>
        public async System.Threading.Tasks.Task<List<Label>> ApiV1LabelsGetAsync(bool? operational = default(bool?), string task = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<List<Label>> localVarResponse = await ApiV1LabelsGetWithHttpInfoAsync(operational, task, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a list of labels Retrieves a simple list of label entities.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="operational">Retrieve only operational labels (optional)</param>
        /// <param name="task">Classification type (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (List&lt;Label&gt;)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<List<Label>>> ApiV1LabelsGetWithHttpInfoAsync(bool? operational = default(bool?), string task = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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

            if (operational != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "operational", operational));
            }
            if (task != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.SocialMedia.Client.ClientUtils.ParameterToMultiMap("", "task", task));
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

            var localVarResponse = await this.AsynchronousClient.GetAsync<List<Label>>("/api/v1/labels", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1LabelsGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a single label Retrieves a single label with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <returns>Label</returns>
        public Label ApiV1LabelsLabelIdGet(int labelId)
        {
            Abp.SocialMedia.Client.ApiResponse<Label> localVarResponse = ApiV1LabelsLabelIdGetWithHttpInfo(labelId);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single label Retrieves a single label with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <returns>ApiResponse of Label</returns>
        public Abp.SocialMedia.Client.ApiResponse<Label> ApiV1LabelsLabelIdGetWithHttpInfo(int labelId)
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

            localVarRequestOptions.PathParameters.Add("label_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(labelId)); // path parameter

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
            var localVarResponse = this.Client.Get<Label>("/api/v1/labels/{label_id}", localVarRequestOptions, this.Configuration);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1LabelsLabelIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

        /// <summary>
        /// Gets a single label Retrieves a single label with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Label</returns>
        public async System.Threading.Tasks.Task<Label> ApiV1LabelsLabelIdGetAsync(int labelId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.SocialMedia.Client.ApiResponse<Label> localVarResponse = await ApiV1LabelsLabelIdGetWithHttpInfoAsync(labelId, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Gets a single label Retrieves a single label with the ID provided in the path.
        /// </summary>
        /// <exception cref="Abp.SocialMedia.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="labelId">ID of the label</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Label)</returns>
        public async System.Threading.Tasks.Task<Abp.SocialMedia.Client.ApiResponse<Label>> ApiV1LabelsLabelIdGetWithHttpInfoAsync(int labelId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
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

            localVarRequestOptions.PathParameters.Add("label_id", Abp.SocialMedia.Client.ClientUtils.ParameterToString(labelId)); // path parameter

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

            var localVarResponse = await this.AsynchronousClient.GetAsync<Label>("/api/v1/labels/{label_id}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("ApiV1LabelsLabelIdGet", localVarResponse);
                if (_exception != null) throw _exception;
            }

            return localVarResponse;
        }

    }
}
