using Abp.Domain.Uow;
using Abp.UI;
using Ermes.Activities;
using Ermes.Categories;
using Ermes.Enums;
using Ermes.Helpers;
using Ermes.Import.Dto;
using Ermes.Net.MimeTypes;
using Ermes.Localization;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ermes.Excel.Common.ExcelCommon;

namespace Ermes.Import
{
    public static class CategoriesImporter
    {
        public const string IndexSheetName = "Index";

        public static async Task<ImportResultDto> ImportCategoriesAsync(string filename, string contentType, CategoryManager categoryManager, ErmesLocalizationHelper localizer, IActiveUnitOfWork context)
        {
            IMultilanguageTable categories;
            ImportResultDto result = new ImportResultDto();
            if (contentType == MimeTypeNames.ApplicationVndMsExcel || contentType == MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet)
            {
                FileInfo finfo = new FileInfo(filename);
                ExcelPackage epack = new ExcelPackage(finfo);
                epack.Compatibility.IsWorksheets1Based = false;
                categories = new ExcelMultilanguageTable(epack.Workbook.Worksheets);
            }
            else
            {
                throw new NotImplementedException();
            }

            bool isFirstSheet = true;
            foreach(IErmesSheet sheet in categories.Sheets)
            {
                if(sheet.Language == IndexSheetName)
                {
                    if (!isFirstSheet)
                        throw new UserFriendlyException(localizer.L("CategoryImportIndexMustBeFirst"));

                    foreach(IErmesRow row in sheet.Rows)
                    {
                        Category cat = await categoryManager.GetCategoryByCodeAsync(row.GetString("Code"));

                        if (cat != null)
                        {
                            result.ElementsUpdated++;
                            string newGroupCode = row.GetString("GroupCode");

                            if (newGroupCode != cat.GroupCode)
                            {
                                // If groupCode has changed, retreive a translation corresponding to the new group, or use the new groupcode as a placeholder
                                categoryManager.CategoriesTranslation
                                    .Where(ct => ct.CoreId == cat.Id)
                                    .ToList()
                                    .ForEach(ct =>
                                    {
                                        ct.Group = categoryManager.CategoriesTranslation
                                            .Where(cx => cx.Core.GroupCode == newGroupCode && cx.Language == ct.Language && cx.Group != cat.GroupCode && cx.Group != newGroupCode)
                                            .FirstOrDefault()?.Group ?? newGroupCode;
                                    });
                            }
                        }
                        else
                        {
                            cat = new Category();
                            result.ElementsAdded++;
                        }

                        cat.Code = row.GetString("Code");
                        cat.GroupCode = row.GetString("GroupCode");
                        cat.Hazard = row.GetEnum<HazardType>("Hazard");
                        cat.MaxValue = row.GetString("Max Value");
                        cat.MinValue = row.GetString("Min Value");
                        cat.Type = row.GetEnum<CategoryType>("Type");
                        cat.GroupIcon = row.GetString("Group Icon");
                        cat.TargetKey = row.GetEnum<TargetType>("Target Key");
                        cat.GroupKey = row.GetString("Group Key");
                        cat.SubGroupKey = row.GetString("SubGroup Key");
                        cat.FieldType = row.GetEnum<FieldType>("Field Type");
                        cat.IsActive = row.GetBoolean("Is Active");

                        await categoryManager.InsertOrUpdateCategoryAsync(cat);
                        context.SaveChanges();
                    }
                }
                else
                {
                    if (!CultureInfo.GetCultures(CultureTypes.AllCultures).Any(c => c.TwoLetterISOLanguageName == sheet.Language))
                        throw new UserFriendlyException(localizer.L("NotALanguageCode", sheet.Language));

                    foreach (IErmesRow row in sheet.Rows)
                    {
                        Category parentCat = await categoryManager.GetCategoryByCodeAsync(row.GetString("Code"));

                        if (parentCat == null)
                            throw new UserFriendlyException(localizer.L("UnexistentEntities", "Category", row.GetString("Code")));

                        CategoryTranslation catTrans = await categoryManager.GetCategoryTranslationByCoreIdLanguageAsync(parentCat.Id, sheet.Language.ToLower());

                        if (catTrans != null)
                        {
                            result.TranslationsUpdated++;
                        }
                        else
                        {
                            catTrans = new CategoryTranslation();
                            result.TranslationsAdded++;
                        }

                        catTrans.Group = row.GetString("Group");
                        catTrans.SubGroup = row.GetString("SubGroup");
                        catTrans.Language = sheet.Language.ToLower();
                        catTrans.Name = row.GetString("Name");
                        catTrans.Values = row.GetStringArray("Values");
                        catTrans.UnitOfMeasure = row.GetString("Unit of Measure");
                        catTrans.Target = row.GetString("Target");
                        catTrans.CoreId = parentCat.Id;

                        // Check if other entities in this language have the same groupcode but a different translation for group, in that case update it
                        categoryManager.CategoriesTranslation
                            .Where(ct => ct.Language == catTrans.Language && ct.Core.GroupCode == parentCat.GroupCode && ct.Group != catTrans.Group)
                            .ToList()
                            .ForEach(ct => ct.Group = catTrans.Group);

                        await categoryManager.InsertOrUpdateCategoryTranslationAsync(catTrans);
                        context.SaveChanges();
                    }
                }
                isFirstSheet = false;
            }

            return result;
        }
    }
}
