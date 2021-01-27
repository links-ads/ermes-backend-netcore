using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    public class PersonActionTracking : PersonAction
    {
        public PersonActionTracking()
        {
            Visibility = VisibilityType.Private;
        }

        [Column(TypeName = "jsonb")]
        public string ExtensionData { get; set; }
    }
}
