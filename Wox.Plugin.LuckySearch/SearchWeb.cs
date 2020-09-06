using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Wox.Plugin.LuckySearch
{
    /// <summary>
    /// Copied from Wox Project (Wox.Plugin/SharedCommands/SearchWeb.cs), because wox.plugin dependency is outdated
    /// </summary>
    public static class SearchWeb
    {

        /// <summary> 
        /// Opens search as a tab in the default browser chosen in Windows settings.
        /// </summary>
        public static void NewTabInBrowser(this string url, string browserPath)
        {
            try
            {
                if (!string.IsNullOrEmpty(browserPath))
                {
                    Process.Start(browserPath, url);
                }
                else
                {
                    Process.Start(url);
                }
            }
            // This error may be thrown for Process.Start(browserPath, url)
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start(url);
            }
        }
    }
}
