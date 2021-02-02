using Abp.Application.Services;
using Ermes.Communications;
using Ermes.GeoJson.Dto;
using Ermes.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ermes.GeoJson
{
    public interface IGeoJsonAppService: IBackofficeApi
    {
        GetGeoJsonCollectionOutput GetFeatureCollection2(GetGeoJsonCollectionInput input);
    }
}
