using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Activations
{
    //This class represents the first responders that have set their status to Active for a certain date
    public class Activation
    {
        public DateTime Timestamp { get; set; }
        public int Counter { get; set; }
        public long[] PersonIdList { get; set; }
    }
}
