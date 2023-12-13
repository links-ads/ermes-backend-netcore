using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Ermes.Web.Utils
{
    public static class UrlHelper
    {
        private static readonly Regex UrlWithProtocolRegex = new Regex("^.{1,10}://.*$");
        public const string HTTP_SSL_SCHEME = "https";
        public const string HTTP_SCHEME = "http";

        public static bool IsRooted(string url)
        {
            if (url.StartsWith("/"))
            {
                return true;
            }

            if (UrlWithProtocolRegex.IsMatch(url))
            {
                return true;
            }

            return false;
        }
    }
}