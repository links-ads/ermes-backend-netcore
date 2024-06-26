/*
 * Importer & Mapper API
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 0.1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mime;
using Abp.Importer.Client;
using Abp.Importer.Model;

namespace Abp.Importer.Api
{

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IDatalakeUtilsApiSync : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Delete Layers
        /// </summary>
        /// <remarks>
        /// Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <returns>List&lt;GeoserverResourceSchema&gt;</returns>
        List<GeoserverResourceSchema> DeleteLayersDeleteLayerDelete(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string));

        /// <summary>
        /// Delete Layers
        /// </summary>
        /// <remarks>
        /// Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <returns>ApiResponse of List&lt;GeoserverResourceSchema&gt;</returns>
        ApiResponse<List<GeoserverResourceSchema>> DeleteLayersDeleteLayerDeleteWithHttpInfo(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string));
        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <remarks>
        /// Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <returns>Object</returns>
        Object GetMetadataMetadataGet(string metadataId);

        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <remarks>
        /// Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object> GetMetadataMetadataGetWithHttpInfo(string metadataId);
        #endregion Synchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IDatalakeUtilsApiAsync : IApiAccessor
    {
        #region Asynchronous Operations
        /// <summary>
        /// Delete Layers
        /// </summary>
        /// <remarks>
        /// Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of List&lt;GeoserverResourceSchema&gt;</returns>
        System.Threading.Tasks.Task<List<GeoserverResourceSchema>> DeleteLayersDeleteLayerDeleteAsync(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Delete Layers
        /// </summary>
        /// <remarks>
        /// Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (List&lt;GeoserverResourceSchema&gt;)</returns>
        System.Threading.Tasks.Task<ApiResponse<List<GeoserverResourceSchema>>> DeleteLayersDeleteLayerDeleteWithHttpInfoAsync(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <remarks>
        /// Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Object</returns>
        System.Threading.Tasks.Task<Object> GetMetadataMetadataGetAsync(string metadataId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Get Metadata
        /// </summary>
        /// <remarks>
        /// Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </remarks>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        System.Threading.Tasks.Task<ApiResponse<Object>> GetMetadataMetadataGetWithHttpInfoAsync(string metadataId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IDatalakeUtilsApi : IDatalakeUtilsApiSync, IDatalakeUtilsApiAsync
    {

    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public partial class DatalakeUtilsApi : IDatalakeUtilsApi
    {
        private Abp.Importer.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatalakeUtilsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public DatalakeUtilsApi() : this((string)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatalakeUtilsApi"/> class.
        /// </summary>
        /// <returns></returns>
        public DatalakeUtilsApi(string basePath)
        {
            this.Configuration = Abp.Importer.Client.Configuration.MergeConfigurations(
                Abp.Importer.Client.GlobalConfiguration.Instance,
                new Abp.Importer.Client.Configuration { BasePath = basePath }
            );
            this.Client = new Abp.Importer.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Abp.Importer.Client.ApiClient(this.Configuration.BasePath);
            this.ExceptionFactory = Abp.Importer.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatalakeUtilsApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public DatalakeUtilsApi(Abp.Importer.Client.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Configuration = Abp.Importer.Client.Configuration.MergeConfigurations(
                Abp.Importer.Client.GlobalConfiguration.Instance,
                configuration
            );
            this.Client = new Abp.Importer.Client.ApiClient(this.Configuration.BasePath);
            this.AsynchronousClient = new Abp.Importer.Client.ApiClient(this.Configuration.BasePath);
            ExceptionFactory = Abp.Importer.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatalakeUtilsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        public DatalakeUtilsApi(Abp.Importer.Client.ISynchronousClient client, Abp.Importer.Client.IAsynchronousClient asyncClient, Abp.Importer.Client.IReadableConfiguration configuration)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (asyncClient == null) throw new ArgumentNullException("asyncClient");
            if (configuration == null) throw new ArgumentNullException("configuration");

            this.Client = client;
            this.AsynchronousClient = asyncClient;
            this.Configuration = configuration;
            this.ExceptionFactory = Abp.Importer.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// The client for accessing this underlying API asynchronously.
        /// </summary>
        public Abp.Importer.Client.IAsynchronousClient AsynchronousClient { get; set; }

        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public Abp.Importer.Client.ISynchronousClient Client { get; set; }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string GetBasePath()
        {
            return this.Configuration.BasePath;
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Abp.Importer.Client.IReadableConfiguration Configuration { get; set; }

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Abp.Importer.Client.ExceptionFactory ExceptionFactory
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
        /// Delete Layers Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <returns>List&lt;GeoserverResourceSchema&gt;</returns>
        public List<GeoserverResourceSchema> DeleteLayersDeleteLayerDelete(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string))
        {
            Abp.Importer.Client.ApiResponse<List<GeoserverResourceSchema>> localVarResponse = DeleteLayersDeleteLayerDeleteWithHttpInfo(datatypeIds, requestCodes, layerName, resourceId);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Delete Layers Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <returns>ApiResponse of List&lt;GeoserverResourceSchema&gt;</returns>
        public Abp.Importer.Client.ApiResponse<List<GeoserverResourceSchema>> DeleteLayersDeleteLayerDeleteWithHttpInfo(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string))
        {
            Abp.Importer.Client.RequestOptions localVarRequestOptions = new Abp.Importer.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Abp.Importer.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = Abp.Importer.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (datatypeIds != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("multi", "datatype_ids", datatypeIds));
            }
            if (requestCodes != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("multi", "request_codes", requestCodes));
            }
            if (layerName != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "layer_name", layerName));
            }
            if (resourceId != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "resource_id", resourceId));
            }

            // authentication (APIKeyHeader) required
            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }

            // make the HTTP request
            var localVarResponse = this.Client.Delete<List<GeoserverResourceSchema>>("/delete_layer", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("DeleteLayersDeleteLayerDelete", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        /// Delete Layers Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of List&lt;GeoserverResourceSchema&gt;</returns>
        public async System.Threading.Tasks.Task<List<GeoserverResourceSchema>> DeleteLayersDeleteLayerDeleteAsync(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.Importer.Client.ApiResponse<List<GeoserverResourceSchema>> localVarResponse = await DeleteLayersDeleteLayerDeleteWithHttpInfoAsync(datatypeIds, requestCodes, layerName, resourceId, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Delete Layers Delete the resource associated to the layer from the datalake  :param datatype_ids: list of datatype_id to filter by, defaults to Query(None) :type datatype_ids: Optional[List[str]], optional :param request_codes: list of codes associated to the map requests :type request_codes: Optional[List[str]], optional :param layer_name: layer name of the resource :type layer_name: Optional[str], optional :param resource_id: id of the resource :type resource_id: Optional[str], optional :param db: DB session instance, defaults to Depends(db_webserver) :type db: Session, optional :return: list of resources deleted :rtype: List[GeoserverResourceSchema]
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="datatypeIds"> (optional)</param>
        /// <param name="requestCodes"> (optional)</param>
        /// <param name="layerName"> (optional)</param>
        /// <param name="resourceId"> (optional)</param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (List&lt;GeoserverResourceSchema&gt;)</returns>
        public async System.Threading.Tasks.Task<Abp.Importer.Client.ApiResponse<List<GeoserverResourceSchema>>> DeleteLayersDeleteLayerDeleteWithHttpInfoAsync(List<string> datatypeIds = default(List<string>), List<string> requestCodes = default(List<string>), string layerName = default(string), string resourceId = default(string), System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {

            Abp.Importer.Client.RequestOptions localVarRequestOptions = new Abp.Importer.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Abp.Importer.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = Abp.Importer.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            if (datatypeIds != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("multi", "datatype_ids", datatypeIds));
            }
            if (requestCodes != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("multi", "request_codes", requestCodes));
            }
            if (layerName != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "layer_name", layerName));
            }
            if (resourceId != null)
            {
                localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "resource_id", resourceId));
            }

            // authentication (APIKeyHeader) required
            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }

            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.DeleteAsync<List<GeoserverResourceSchema>>("/delete_layer", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("DeleteLayersDeleteLayerDelete", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        /// Get Metadata Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <returns>Object</returns>
        public Object GetMetadataMetadataGet(string metadataId)
        {
            Abp.Importer.Client.ApiResponse<Object> localVarResponse = GetMetadataMetadataGetWithHttpInfo(metadataId);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Get Metadata Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <returns>ApiResponse of Object</returns>
        public Abp.Importer.Client.ApiResponse<Object> GetMetadataMetadataGetWithHttpInfo(string metadataId)
        {
            // verify the required parameter 'metadataId' is set
            if (metadataId == null)
            {
                throw new Abp.Importer.Client.ApiException(400, "Missing required parameter 'metadataId' when calling DatalakeUtilsApi->GetMetadataMetadataGet");
            }

            Abp.Importer.Client.RequestOptions localVarRequestOptions = new Abp.Importer.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Abp.Importer.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = Abp.Importer.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "metadata_id", metadataId));

            // authentication (APIKeyHeader) required
            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }

            // make the HTTP request
            var localVarResponse = this.Client.Get<Object>("/metadata", localVarRequestOptions, this.Configuration);
            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetMetadataMetadataGet", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

        /// <summary>
        /// Get Metadata Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of Object</returns>
        public async System.Threading.Tasks.Task<Object> GetMetadataMetadataGetAsync(string metadataId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            Abp.Importer.Client.ApiResponse<Object> localVarResponse = await GetMetadataMetadataGetWithHttpInfoAsync(metadataId, cancellationToken).ConfigureAwait(false);
            return localVarResponse.Data;
        }

        /// <summary>
        /// Get Metadata Gets the layer metadata given the metadata_id.  :param metadata_id: metadata_id of the layer. It is returned by the API /layers :type metadata_id: str
        /// </summary>
        /// <exception cref="Abp.Importer.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="metadataId"></param>
        /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
        /// <returns>Task of ApiResponse (Object)</returns>
        public async System.Threading.Tasks.Task<Abp.Importer.Client.ApiResponse<Object>> GetMetadataMetadataGetWithHttpInfoAsync(string metadataId, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            // verify the required parameter 'metadataId' is set
            if (metadataId == null)
            {
                throw new Abp.Importer.Client.ApiException(400, "Missing required parameter 'metadataId' when calling DatalakeUtilsApi->GetMetadataMetadataGet");
            }


            Abp.Importer.Client.RequestOptions localVarRequestOptions = new Abp.Importer.Client.RequestOptions();

            string[] _contentTypes = new string[] {
            };

            // to determine the Accept header
            string[] _accepts = new string[] {
                "application/json"
            };

            var localVarContentType = Abp.Importer.Client.ClientUtils.SelectHeaderContentType(_contentTypes);
            if (localVarContentType != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
            }

            var localVarAccept = Abp.Importer.Client.ClientUtils.SelectHeaderAccept(_accepts);
            if (localVarAccept != null)
            {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
            }

            localVarRequestOptions.QueryParameters.Add(Abp.Importer.Client.ClientUtils.ParameterToMultiMap("", "metadata_id", metadataId));

            // authentication (APIKeyHeader) required
            if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-API-Key")))
            {
                localVarRequestOptions.HeaderParameters.Add("X-API-Key", this.Configuration.GetApiKeyWithPrefix("X-API-Key"));
            }

            // make the HTTP request
            var localVarResponse = await this.AsynchronousClient.GetAsync<Object>("/metadata", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

            if (this.ExceptionFactory != null)
            {
                Exception _exception = this.ExceptionFactory("GetMetadataMetadataGet", localVarResponse);
                if (_exception != null)
                {
                    throw _exception;
                }
            }

            return localVarResponse;
        }

    }
}
