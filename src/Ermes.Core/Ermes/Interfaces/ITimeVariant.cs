using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Interfaces
{
    public interface ITimeVariant
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }
}
