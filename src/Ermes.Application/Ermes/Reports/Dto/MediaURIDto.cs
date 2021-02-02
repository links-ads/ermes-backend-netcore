using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Reports.Dto
{
    public class MediaURIDto
    {
        public string MediaURI { get; set; }
        public string ThumbnailURI { get; set; }
        public MediaType MediaType { get; set; }
    }
}
