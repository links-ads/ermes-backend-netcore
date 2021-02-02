using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto.Datatable
{
    public interface IDTParameters
    {
        //DTColumn[] Columns { get; set; }

        DTSearch Search { get; set; }

        List<DTOrder> Order { get; set; }
    }
}
