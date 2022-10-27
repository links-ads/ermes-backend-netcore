using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Tests.Geometry
{
    public class Test_FeatureCollectionBase
    {
        public List<Test_FeatureItem> Features { get; set; }
        public string Type { get; set; } = "FeatureCollection";
    }

    public class Test_FeatureItem
    {
        public string Type { get; set; }
        public object Geometry { get; set; }
        public Test_FeatureProperty Properties { get; set; }
    }

    public class Test_FeatureProperty
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
    }
}
