using Abp.ErmesSocialNetCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Social.Dto
{
    public class SocialItemOutput<T>
    {
        public T Item { get; set; }
    }
}
