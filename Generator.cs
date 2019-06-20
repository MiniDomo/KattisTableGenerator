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
                string url = folder.Url;
                if (Regex.IsMatch (url, @"^https://github.com/[^/]+/[^/]+/?$")) {
                    if (!url.EndsWith ('/'))
                        url += '/';
                    url += "blob/master/";
                } else {
                    Match match = Regex.Match (url, "^https://github.com/[^/]+/[^/]+/tree/");
                    string res = match.Value;
                    url = res.Substring (0, res.Length - 5) + "blob/" + url.Substring (res.Length);
                }
                Console.WriteLine (url);
                while (folder.Count > 0) {
                    string path = folder.Next ();
                    DirectoryInfo dir = new DirectoryInfo (path);
                    foreach (FileInfo info in dir.GetFiles ()) {
                        Console.WriteLine (info.FullName);
                        if (Regex.IsMatch (info.Name, @"^[A-Za-z\d]+\.[A-Za-z\d]+$")) {
                            int pos = info.Name.IndexOf ('.', StringComparison.Ordinal);
                            string id = info.Name.Substring (0, pos);
                            string ext = info.Name.Substring (pos + 1);

                        }
                    }
                    foreach (DirectoryInfo info in dir.GetDirectories ()) {
                        Console.WriteLine (info.FullName);

                    }
                }
            }
        }
    }
}