using Ermes.Activities;
using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    public class PersonActionActivity : PersonAction
    {
        public PersonActionActivity()
        {
        }

        [ForeignKey("ActivityId")]
        public virtual Activity Activity { get; set; }
        public virtual int ActivityId { get; set; }
    }
}
