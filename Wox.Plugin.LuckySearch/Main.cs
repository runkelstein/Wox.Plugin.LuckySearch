using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;

namespace Wox.Plugin.LuckySearch
{
    public class Main : IPlugin
    {
        private string _pluginDirectory;
        
        public List<Result> Query(Query query)
        {
            var result = new Result
            {
                Title = "Lucky Search",
                SubTitle = $"Query: {query.Search}",
                IcoPath = Path.Combine(_pluginDirectory, "Images", "google.png"),
                Action = arg =>
                {
                    OnQueryExecute(arg, query);
                    return true;
                } 
            };
            return new List<Result> {result};
        }
        
        public void Init(PluginInitContext context)
        {
            _pluginDirectory = context.CurrentPluginMetadata.PluginDirectory;
        }

        /// <summary>
        /// Immediately executes a search query, opens the system browser
        /// with the first page found by google
        /// </summary>
        private async void OnQueryExecute(ActionContext arg, Query query)
        {

            var web = new HtmlWeb();
            var searchUrl =  BuildSearchUrl(query);
            var doc = await web.LoadFromWebAsync(searchUrl);
            
            var targetUrl = doc.DocumentNode.SelectSingleNode("//a[./h3]")?.Attributes["href"].Value;

            if (SanitizeUrl(targetUrl, out var sanitizedUrl))
            {
                sanitizedUrl.NewTabInBrowser(null);
            }

        }
        
        /// <remarks>
        /// query parameters:
        /// num = 1 -- only one result will be returned
        /// safe=active -- only safe for work results are returned
        /// gbv= 1 -- most basic google page version is returned, less bandwith is required
        /// q -- the search query
        /// </remarks>
        private static string BuildSearchUrl(Query query)
        {
            return $"https://www.google.com/search?q=${query.Search}&num=1&safe=active&gbv=1";
        }

        /// <summary>
        /// remove google url forwarding
        /// </summary>
        /// <param name="targetUrl">The target url extracted from the google search page</param>
        /// <param name="sanitizedUrl">The sanitized url</param>
        /// <returns>
        /// true if sanitizing worked, false otherwise
        /// </returns>
        private static bool SanitizeUrl(string targetUrl, out string sanitizedUrl)
        {
            sanitizedUrl = null;
            if (targetUrl == null)
            {
                return false;
            }
            
            var startIdx = 7; // remove url?q=
            var endIdx = targetUrl.IndexOf("&amp;", StringComparison.Ordinal) - startIdx;
            if (endIdx < 1)
            {
                return true;
            }

            sanitizedUrl = targetUrl.Substring(startIdx, endIdx);
            return !string.IsNullOrEmpty(sanitizedUrl);
        }
        

    }
}