using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ermes.Persons
{
    public class PersonActionSharingPosition : PersonAction
    {
        public PersonActionSharingPosition()
        {
            Visibility = VisibilityType.Private;
        }
    }
}