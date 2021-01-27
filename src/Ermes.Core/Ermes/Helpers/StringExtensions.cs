using Abp.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Helpers
{
    public static class StringExtensions
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T TryParseEnum<T>(this string value)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (Exception)
            {
                throw new UserFriendlyException(string.Format("Invalid value: {0}", value));
            }
        }
    }
}
