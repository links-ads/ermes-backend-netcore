using System;

namespace Ermes.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ErmesIgnoreApiAttribute : Attribute
    {
        public ErmesIgnoreApiAttribute(bool ignoreApi)
        {
            IgnoreApi = ignoreApi;
        }

        public bool IgnoreApi { get; set; }
    }
}
