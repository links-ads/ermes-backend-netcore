using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.ExternalServices.Csi
{
    public interface ICsiManager
    {
        Task<int> SearchVolontarioAsync(string taxCode, long personId);
        Task<int> InsertInterventiVolontariAsync(
            long personId, 
            int personLegaycId, 
            double? latitude, 
            double? longitude, 
            string activity, 
            DateTime timestamp, 
            string status, 
            int? operationId = null
        );
        Task InserisciFromFaster(
            long personId,
            int personLegaycId,
            int reportId,
            string creator,
            double latitude,
            double longitude,
            string description,
            string hazard,
            string status,
            List<string> mediaURIs
        );
    }
}
