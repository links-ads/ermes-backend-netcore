using System;

namespace Ermes.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ErmesApiKeyAttribute : Attribute, IErmesApiKeyAttribute
    {
    }
}
