using Ermes.Enums;
using Ermes.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    public class PersonActionStatus : PersonAction
    {
        public PersonActionStatus()
        {
            Visibility = VisibilityType.Private;
        }

        [Column("Status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { Status = value.ParseEnum<ActionStatusType>(); }
        }

        [NotMapped]
        public ActionStatusType Status { get; set; }
    }
}
