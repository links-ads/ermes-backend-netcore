using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Ermes.CompetenceAreas;
using Ermes.Enums;
using Ermes.Interfaces;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Valid;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ermes.Jobs
{
    [Serializable]
    public class ImportCompetenceAreasJobArgs
    {
        public string Filename = null;
        public CompetenceAreaType CompetenceAreaType { get; set; }
    }

    public class ImportCompetenceAreasJob : BackgroundJob<ImportCompetenceAreasJobArgs>, ITransientDependency
    {
        private readonly IAppFolders _appFolders;
        private readonly CompetenceAreaManager _compAreaManager;
        private readonly string[] AcceptedGeographies = { "Polygon", "MultiPolygon" };

        public ImportCompetenceAreasJob(IAppFolders appFolders, CompetenceAreaManager compAreaManager)
        {
            _appFolders = appFolders;
            _compAreaManager = compAreaManager;
        }

        [UnitOfWork]
        public override void Execute(ImportCompetenceAreasJobArgs args)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var tempFilePath = Path.Combine(_appFolders.TempFileDownloadFolder, args.Filename);
                var fileString = string.Empty;
                try
                {
                    using var fsTempfile = new StreamReader(tempFilePath);
                    fileString = fsTempfile.ReadToEnd();
                }
                catch(Exception)
                {
                    Logger.ErrorFormat("File not found");
                    return;
                }

                Dictionary<string, List<string>> uuidDictionary = _compAreaManager.GetUuidDictionary();
                GeoJsonReader reader = new GeoJsonReader();
                FeatureCollection featCollection = reader.Read<FeatureCollection>(fileString);
                foreach (var item in featCollection)
                {
                    try
                    {
                        var uuid = item.Attributes["uuid"].ToString();
                        var source = item.Attributes["ente_for"].ToString();
                        if (item.Geometry.IsEmpty || !item.Geometry.IsValid || !AcceptedGeographies.Contains(item.Geometry.GeometryType))
                        {
                            Logger.ErrorFormat("Invalid Geometry with uuid {0} from {1}", uuid, source);
                            var isValidOp = new IsValidOp(item.Geometry);
                            if (!isValidOp.IsValid)
                                Logger.ErrorFormat("Error Message: {0}, Coordinates: {1}", isValidOp.ValidationError.Message, isValidOp.ValidationError.Coordinate);
                            else
                                Logger.ErrorFormat("Invalid Geometry: {0}", item.Geometry.AsText());

                            continue;
                        }
                        else
                        {
                            if (uuidDictionary.ContainsKey(source))
                            {
                                var list = uuidDictionary[source];
                                if (list == null)
                                    list = new List<string>();

                                if (!list.Contains(uuid))
                                {
                                    list.Add(uuid);
                                    uuidDictionary[source] = list;
                                }
                                else
                                {
                                    Logger.WarnFormat("Element {0} from {1} already prensent", uuid, source);
                                    continue;
                                }
                            }
                            else
                                uuidDictionary.Add(source, new List<string>() { uuid });
                        }

                        var nameList = item.Attributes.GetNames();
                        var valueList = item.Attributes.GetValues();
                        var metadata = new Dictionary<string, string>();
                        var index = 0;
                        foreach (var key in nameList)
                        {
                            metadata.Add(key, valueList[index++].ToString());
                        }
                        var newCompArea = new CompetenceArea()
                        {
                            AreaOfInterest = item.Geometry,
                            CompetenceAreaType = args.CompetenceAreaType,
                            Metadata = JsonConvert.SerializeObject(metadata),
                            Uuid = uuid,
                            Name = item.Attributes[GetNameByCompetenceType(args.CompetenceAreaType)].ToString(),
                            Source = source
                        };

                        _compAreaManager.InsertCompetenceArea(newCompArea);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Exception: {0}", e.Message);
                    }

                }
                //Delete file from Temp folder
                try
                {
                    File.Delete(tempFilePath);
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat("File deletion failed: {0}", e.Message);
                }

                Logger.InfoFormat("Import Competence area job ended at {0}", DateTime.Now);

            }
        }

        private string GetNameByCompetenceType(CompetenceAreaType type)
        {
            string res = type switch
            {
                CompetenceAreaType.Municipality => "comune_nom",
                CompetenceAreaType.Province => "nome",
                CompetenceAreaType.Region => "region_nom",
                _ => "nome",
            };
            return res;
        }
    }
}
