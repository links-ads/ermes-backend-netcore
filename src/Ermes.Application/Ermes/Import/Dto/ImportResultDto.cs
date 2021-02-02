using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Import.Dto
{
    public class ImportResultDto
    {
        public int TranslationsAdded {get; set;} = 0;
        public int TranslationsUpdated {get; set;} = 0;
        public int TranslationsDetectedEmptyAndSkipped {get; set;} = 0;
        public int ElementsAdded {get; set;} = 0;
        public int ElementsUpdated {get; set;} = 0;
    }
}
