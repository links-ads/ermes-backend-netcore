﻿using Ermes.Dto.Spatial;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.GeoJson.Dto
{
    public class GetGeoJsonCollectionInput
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PointPosition NorthEastBoundary { get; set; }
        public PointPosition SouthWestBoundary { get; set; }
        public List<EntityType> EntityTypes { get; set; }
        public List<ActionStatusType> StatusTypes { get; set; }
        public List<int> ActivityIds { get; set; }
    }

    public class GeoJsonItem
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }

    }

    public class GetGeoJsonCollectionOutput
    {
        public List<FeatureDto<GeoJsonItem>> Features { get; set; } = new List<FeatureDto<GeoJsonItem>>();
        public string Type { get; set; } = "FeatureCollection";
    }
}
