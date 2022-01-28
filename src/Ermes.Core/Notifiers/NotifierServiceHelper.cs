using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Notifiers.MessageBody;
using NetTopologySuite.IO;
using System;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class NotifierServiceHelper: ErmesDomainServiceBase
    {
        private readonly MapRequestManager _mapRequestManager;

        public NotifierServiceHelper(MapRequestManager mapRequestManager)
        {
            _mapRequestManager = mapRequestManager;
        }

        public async Task<Tuple<string, string>> GetEntityByIdAsync(EntityType type, int entityId)
        {
            string entityIdentifier = "", payload = "";
            var writer = new GeoJsonWriter();

            switch (type)
            {
                case EntityType.MapRequest:
                    var mr = await _mapRequestManager.GetMapRequestByIdAsync(entityId);
                    entityIdentifier = mr.Code;
                    MapRequestBody body = new MapRequestBody()
                    {
                        hazard = mr.HazardString.ToLower(),
                        delineation_time_start = mr.Duration.LowerBound,
                        delineation_time_end = mr.Duration.UpperBound,
                        request_code = mr.Code,
                        geometry = mr.AreaOfInterest
                    };
                    payload = writer.Write(body);
                    
                    break;
                default:
                    break;
            }

            return new Tuple<string, string>(payload, entityIdentifier);
        }
    }
}
