using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Notifiers.MessageBody;
using NetTopologySuite.IO;
using System;
using System.Linq;
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
                    int numOfLayers = mr.MapRequestLayers.Count;
                    payloads = new string[numOfLayers];
                    dataTypeIds = new int[numOfLayers];
                    entityIdentifier = mr.Code;
                    for (int i = 0; i < numOfLayers; i++)
                    {
                        MapRequestBody body = new MapRequestBody()
                        {
                            start = mr.Duration.LowerBound,
                            end = mr.Duration.UpperBound,
                            request_code = mr.Code,
                            geometry = mr.AreaOfInterest,
                            frequency = mr.Frequency,
                            datatype_id = mr.MapRequestLayers.ElementAt(i).LayerDataTypeId,
                            resolution = mr.Resolution
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
