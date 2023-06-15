using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Enums;
using Ermes.Import.Dto;
using Ermes.Layers;
using Ermes.Localization;
using Ermes.Net.MimeTypes;
using OfficeOpenXml;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public static class LayersImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportLayersAsync(string filename, string contentType, LayerManager layerManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable layers;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                layers = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach (IErmesSheet sheet in layers.Sheets)
            {
                if (sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("LayerImportIndexMustBeFirst"));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Layer layer = await layerManager.GetLayerByDataTypeIdAsync(row.GetInt("DataTypeId").Value);

                        if (layer != null)
                            result.ElementsUpdated++;
                        else
                        {
                            layer = new Layer();
                            result.ElementsAdded++;
                        }

                        layer.DataTypeId = row.GetInt("DataTypeId").Value;
                        layer.GroupKey = row.GetString("Group Key");
                        layer.SubGroupKey = row.GetString("SubGroup Key");
                        layer.PartnerName = row.GetString("Partner Name");
                        layer.Format = row.GetEnum<FormatType>("Format");
                        layer.CanBeVisualized = row.GetBoolean("Can be visualized");
                        layer.IsActive = row.GetBoolean("Is Active");
                        layer.Frequency = row.GetEnum<FrequencyType>("Update Frequency");
                        layer.UnitOfMeasure = row.GetString("Unit of measure");
                        layer.Order = row.GetInt("Order").Value;
                        layer.ParentDataTypeId = row.GetInt("Parent DataTypeId");
                        await layerManager.InsertOrUpdateLayerAsync(layer);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Layer parent = await layerManager.GetLayerByDataTypeIdAsync(row.GetInt("DataTypeId").Value);

                        if (parent == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Layer", row.GetInt("DataTypeId").Value));

                        LayerTranslation trans = await layerManager.GetLayerTranslationByCoreIdLanguageAsync(parent.Id, sheet.Language.ToLower());

                        if (trans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            trans = new LayerTranslation();
                            result.TranslationsAdded++;
                        }

                        trans.Group = row.GetString("Group");
                        trans.SubGroup = row.GetString("SubGroup");
                        trans.Name = row.GetString("Name");
                        trans.Language = sheet.Language.ToLower();
                        trans.CoreId = parent.Id;

                        await layerManager.InsertOrUpdateLayerTranslationAsync(trans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
