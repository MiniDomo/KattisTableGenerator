using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KattisTableGenerator {
    public class Generator {
        private HashSet<string> ignored, urls;
        private Stack<Folder> folders;
        public Generator (Config config) {
            folders = config.Folders;
            ignored = config.Ignored;
            urls = config.Urls;
        }

        public void checkFolders () {
            while (folders.Count > 0) {
                Folder folder = folders.Pop ();
                string url = AdjustUrl (folder.Url);
                Console.WriteLine (url);
                while (folder.Count > 0) {
                    string path = folder.Next ();
                    DirectoryInfo dir = new DirectoryInfo (path);
                    HandleFiles (dir, url);
                    HandleFolders (dir, url);
                }
            }
        }
        /* 
        FOLDERS
        files must have problem id as name
        folders must have problem id as name (change url)
        - subsequent files must have problem id or main as name
        - recursive

        URLS
        files must have problem id as name
        folders must have problem id as name (change url)
        - subsequent files must have problem id or main as name
        - recursive
         */

        private string AdjustUrl (string url) {
            string res = url;
            if (Regex.IsMatch (res, @"^https://github.com/[^/]+/[^/]+/?$")) {
                if (!res.EndsWith ('/'))
                    res += '/';
                res += "blob/master/";
            } else {
                Match match = Regex.Match (res, "^https://github.com/[^/]+/[^/]+/tree/");
                string val = match.Value;
                res = val.Substring (0, val.Length - 5) + "blob/" + res.Substring (val.Length) + '/';
            }
            return res;
        }

        private void HandleFiles (DirectoryInfo dir, string url) {
            foreach (FileInfo info in dir.GetFiles ()) {

                if (!ignored.Contains (info.Name) && Regex.IsMatch (info.Name, @"^[A-Za-z\d]+\.[A-Za-z\d]+$")) {
                    int pos = info.Name.IndexOf ('.', StringComparison.Ordinal);
                    string id = info.Name.Substring (0, pos);
                    string ext = info.Name.Substring (pos + 1);
                    if (!ignored.Contains (id) && !ignored.Contains ('.' + ext) && Mapping.ContainsKey (id)) {
                        Console.WriteLine (info.FullName);
                    }
                }
            }
        }

        private void HandleFolders (DirectoryInfo dir, string url) {
            foreach (DirectoryInfo info in dir.GetDirectories ()) {

                if (!ignored.Contains (info.Name) && Mapping.ContainsKey (info.Name)) {
                    Console.WriteLine (info.FullName);
                }
            }
        }
    }
}