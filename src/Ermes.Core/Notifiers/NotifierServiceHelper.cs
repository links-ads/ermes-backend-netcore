using Abp.ObjectMapping;
using Ermes.Enums;
using Ermes.MapRequests;
using Ermes.Notifiers.MessageBody;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
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
                        var body = new MapRequestBody()
                        {
                            start = mr.Duration.LowerBound,
                            end = mr.Duration.UpperBound,
                            request_code = mr.Code,
                            geometry = mr.AreaOfInterest,
                            datatype_id = mr.MapRequestLayers.ElementAt(i).LayerDataTypeId,
                            title = mr.Title
                        };

                        switch (mr.Type)
                        {
                            case MapRequestType.FireAndBurnedArea:
                                var typedBody1 = ObjectMapper.Map<MapRequestFireAndBurnedAreaBody>(body);
                                typedBody1.frequency = mr.Frequency;
                                typedBody1.resolution = mr.Resolution;
                                payloads[i] = writer.Write(typedBody1);
                                break;
                            case MapRequestType.PostEventMonitoring:
                                payloads[i] = writer.Write(body);
                                break;
                            case MapRequestType.WildfireSimulation:
                                var typedBody2 = ObjectMapper.Map<MapRequestWildFireSimulationBody>(body);
                                typedBody2.frequency = mr.Frequency;
                                typedBody2.resolution = mr.Resolution;
                                typedBody2.description = mr.Description;
                                typedBody2.do_spotting = mr.DoSpotting;
                                typedBody2.time_limit = mr.TimeLimit;
                                typedBody2.probabilityRange = mr.ProbabilityRange;
                                typedBody2.boundary_conditions = ObjectMapper.Map<List<BoundaryConditionBody>>(mr.BoundaryConditions);
                                payloads[i] = writer.Write(typedBody2);
                                break;
                            default:
                                break;
                        }

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
