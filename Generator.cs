using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace KattisTableGenerator {
    public class Generator {
        private HashSet<string> ignored, urls;
        private Stack<Folder> folders;
        private SortedList<string, KattisProblem> table;
        private HtmlWeb web;
        public Generator (Config config) {
            folders = config.Folders;
            ignored = config.Ignored;
            urls = config.Urls;
            table = new SortedList<string, KattisProblem> ();
            web = new HtmlWeb ();
        }

        public string GetTableString () {
            List<KattisProblem> list = new List<KattisProblem> ();
            foreach (var pair in table) {
                list.Add (pair.Value);
            }
            list.Sort ();
            string res = string.Empty;
            foreach (var a in list)
                res += a + "\n";
            return res;
        }

        public void checkFolders () {
            while (folders.Count > 0) {
                Folder folder = folders.Pop ();
                string url = AdjustUrl (folder.Url);
                Console.WriteLine (url);
                while (folder.Count > 0) {
                    string path = folder.Next ();
                    DirectoryInfo dir = new DirectoryInfo (path);
                    HandleFiles (dir, url, false);
                    HandleFolders (dir, url);
                }
            }
        }

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

        private void HandleFiles (DirectoryInfo dir, string url, bool folderIsID) {
            foreach (FileInfo info in dir.GetFiles ()) {
                if (!ignored.Contains (info.Name) && Regex.IsMatch (info.Name, @"^[A-Za-z\d]+\.[A-Za-z\d]+$")) {
                    int pos = info.Name.IndexOf ('.', StringComparison.Ordinal);
                    string id = info.Name.Substring (0, pos);
                    string ext = info.Name.Substring (pos + 1);
                    string fullUrl = url + info.Name;
                    if (!ignored.Contains (id) && !ignored.Contains ('.' + ext) && (folderIsID == id.Equals ("main") || folderIsID == id.Equals (dir.Name)))
                        TryAdd (fullUrl, folderIsID ? dir.Name : id, ext);
                }
            }
        }

        private void HandleFolders (DirectoryInfo dir, string url) {
            foreach (DirectoryInfo info in dir.GetDirectories ()) {
                if (!ignored.Contains (info.Name)) {
                    HandleFiles (info, url + info.Name + "/", true);
                }
            }
        }

        private void TryAdd (string url, string id, string ext) {
            if (!ValidExtensions.Contains (ext))
                return;
            if (table.ContainsKey (id)) {
                KattisProblem problem = table[id];
                string lang = ValidExtensions.Get (ext);
                if (!problem.Contains (lang))
                    problem.Add (lang, url);
            } else {
                string name = string.Empty;
                if (Mapping.ContainsKey (id)) {
                    name = Mapping.Get (id);
                } else {
                    Stopwatch a = new Stopwatch ();
                    a.Start ();
                    HtmlDocument doc = web.Load ("https://open.kattis.com/problems/" + id);
                    Console.Write (a.Elapsed.Milliseconds + " ");
                    HtmlNode node = doc.DocumentNode.SelectSingleNode ("//head/title");
                    a.Stop ();
                    Console.Write (a.Elapsed.Milliseconds + " ");
                    Match match = Regex.Match (node.InnerHtml, @"(.+) &ndash; Kattis, Kattis");
                    name = WebUtility.HtmlDecode (match.Groups[1].ToString ());
                    Console.Write (name + "\n");
                }
                if (!string.IsNullOrEmpty (name) && !name.Equals ("404: Not Found")) {
                    table.Add (id, new KattisProblem (name, id));
                    KattisProblem problem = table[id];
                    string lang = ValidExtensions.Get (ext);
                    if (!problem.Contains (lang))
                        problem.Add (lang, url);
                }
            }
        }
    }
}