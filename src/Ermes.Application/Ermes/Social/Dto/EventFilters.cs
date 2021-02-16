using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public class EventFilters : SocialPaginationInput
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<int> Hazards { get; set; }
        public List<float> SouthWest { get; set; }
        public List<float> NorthEast { get; set; }
    }
}
