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

        public async Task<Tuple<string[], string, int[]>> GetPayloadsByEntityIdAsync(EntityType type, int entityId)
        {
            string entityIdentifier = "";
            string[] payloads = null;
            int[] dataTypeIds = null;
            var writer = new GeoJsonWriter();

            switch (type)
            {
                case EntityType.MapRequest:
                    var mr = await _mapRequestManager.GetMapRequestByIdAsync(entityId);
                    payloads = new string[mr.DataTypeIds.Count];
                    dataTypeIds = new int[mr.DataTypeIds.Count];
                    entityIdentifier = mr.Code;
                    for (int i = 0; i < mr.DataTypeIds.Count; i++)
                    {
                        MapRequestBody body = new MapRequestBody()
                        {
                            start = mr.Duration.LowerBound,
                            end = mr.Duration.UpperBound,
                            request_code = mr.Code,
                            geometry = mr.AreaOfInterest,
                            frequency = mr.Frequency,
                            datatype_id = mr.DataTypeIds[i]
                        };
                        payloads[i] = writer.Write(body);
                        dataTypeIds[i] = body.datatype_id;
                    }
                    
                    break;
                default:
                    break;
            }

            return new Tuple<string[], string, int[]>(payloads, entityIdentifier, dataTypeIds);
        }
    }
}
