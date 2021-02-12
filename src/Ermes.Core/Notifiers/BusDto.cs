using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifiers
{
    public class BusDto<T>
    {
        public EntityType EntityType { get; set; }
        public EntityWriteAction EntityWriteAction { get; set; }
        public T Content { get; set; }
    }
}
