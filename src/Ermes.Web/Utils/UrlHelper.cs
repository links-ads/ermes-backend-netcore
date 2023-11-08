using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Ermes.Web.Utils
{
    public static class UrlHelper
    {
        private static readonly Regex UrlWithProtocolRegex = new Regex("^.{1,10}://.*$");

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

        public static string GetSchemeFromRequest(HttpRequest request)
        {
            string scheme = "http";
            if (request.Headers != null && request.Headers.Count > 0 && request.Headers.ContainsKey("X-Forwarded-Proto"))
                scheme = request.Headers["X-Forwarded-Proto"];

            return scheme;
        }
    }
}