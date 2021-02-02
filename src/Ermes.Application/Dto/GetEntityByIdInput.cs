using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Dto
{
    public class GetEntityByIdInput<T>
    {
        public T Id { get; set; }
        public bool IncludeArea { get; set; }
    }
}
