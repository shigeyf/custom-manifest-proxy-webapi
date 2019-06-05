using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;


namespace Custom_Manifest_Proxy
{
    public static class HlsCustomManifestProxy
    {
        [FunctionName("HlsCustomManifestProxy")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string encodedAuthToken = req.Query["token"];
            string encodedManifestUrl = req.Query["manifest"];
            string decodedManifestUrl = WebUtility.UrlDecode(encodedManifestUrl);
            Uri manifestUri = new Uri(decodedManifestUrl);
            string baseManifestUri = manifestUri.Scheme + "://" + manifestUri.Host;
            for (int i = 0; i < manifestUri.Segments.Length; i++)
            {
                baseManifestUri += manifestUri.Segments[i];
                if (Regex.Match(manifestUri.Segments[i], @".ism\/$").Success)
                    break;
            }

            //const string manifestRegex = @"(QualityLevels\(\d+\)\/Manifest\([\w\d=-]+,[\w\d=-]+\))";
            const string manifestRegex = @"(QualityLevels\(\d+\)\/Manifest\(.+\))";
            const string qualityLevelRegex = @"(QualityLevels\(\d+\))";
            const string fragmentsRegex = @"(Fragments\([\w\d=-]+,[\w\d=-]+\))";
            const string urlRegex = @"("")(https?:\/\/[\da-z\.-]+\.[a-z\.]{2,6}[\/\w \.-]*\/?[\?&][^&=]+=[^&=#]*)("")";
            string manifest = null;

            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetStringAsync(decodedManifestUrl);
                    manifest = await responseTask;

                    var modManifest = manifest;

                    // Convert links of 2nd level M3U8 playlst
                    modManifest = Regex.Replace(modManifest, manifestRegex, m => string.Format(CultureInfo.InvariantCulture, "HlsCustomManifestProxy?manifest={0}&token={1}", WebUtility.UrlEncode(baseManifestUri + m.Value), encodedAuthToken));

                    // Convert Key Delivery URL with AuthToken
                    modManifest = Regex.Replace(modManifest, urlRegex, string.Format(CultureInfo.InvariantCulture, "$1$2&token={0}$3", encodedAuthToken));

                    // Convert fragment URLs to add Base URL in only 2nd level M3U8 playlist
                    var match = Regex.Match(manifestUri.ToString(), qualityLevelRegex);
                    if (match.Success)
                    {
                        var qualityLevel = match.Groups[0].Value;
                        modManifest = Regex.Replace(modManifest, fragmentsRegex, m => string.Format(CultureInfo.InvariantCulture, baseManifestUri + qualityLevel + "/" + m.Value));
                    }

                    manifest = modManifest;
                }
            }
            finally
            {
            }

            return manifest != null
                ? (ActionResult)new ContentResult { Content = manifest, ContentType = "application/vnd.apple.mpegurl" }
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

    }
}
