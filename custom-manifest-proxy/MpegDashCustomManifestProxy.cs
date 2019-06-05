using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Custom_Manifest_Proxy
{
    public static class MpegDashCustomManifestProxy
    {
        [FunctionName("MpegDashCustomManifestProxy")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string encodedManifestUrl = req.Query["manifest"];
            string decodedManifestUrl = WebUtility.UrlDecode(encodedManifestUrl);
            Uri manifestUri = new Uri(decodedManifestUrl);
            string baseManifestUri = manifestUri.Scheme + "://" + manifestUri.Host;
            for (int i = 0; i < manifestUri.Segments.Length - 1; i++)
            {
                baseManifestUri += manifestUri.Segments[i];
            }

            string manifest = null;

            using (var client = new HttpClient())
            {
                var responseTask = client.GetStringAsync(decodedManifestUrl);
                manifest = await responseTask;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(manifest);
                XmlNamespaceManager nsmanager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmanager.AddNamespace("x", "urn:mpeg:dash:schema:mpd:2011");
                XmlNode mpd = xmlDoc.SelectSingleNode("x:MPD", nsmanager);

                // Any manifest manipulation/modification can be here
                // ------
                // ------

                // Change BaseURL
                XmlNode baseUrl = xmlDoc.CreateNode(XmlNodeType.Element, "BaseURL", "urn:mpeg:dash:schema:mpd:2011");
                baseUrl.InnerText = baseManifestUri;
                mpd.AppendChild(baseUrl);

                // Convert XmlDocument to String
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlDoc.WriteTo(xmlTextWriter);
                manifest = stringWriter.ToString();
            }

            return manifest != null
                ? (ActionResult)new ContentResult { Content = manifest, ContentType = "application/dash+xml" }
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
