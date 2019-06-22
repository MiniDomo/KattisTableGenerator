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
            foreach (var pair in table)
                list.Add (pair.Value);
            list.Sort ();
            string res = string.Format ("| {0} Problem{1} | Languages |\n| - | - |\n", list.Count, list.Count == 1 ? "" : "s");
            foreach (KattisProblem problem in list)
                res += problem + "\n";
            res = res.Trim ();
            return res;
        }

        public void ProcessConfig () {
            CheckFolders ();
            CheckUrls ();
        }

        private void CheckUrls () {
            foreach (string url in urls) {
                HtmlDocument doc = web.Load (url);
                HtmlNodeCollection collection = doc.DocumentNode.SelectNodes ("//*[@class=\"js-navigation-open\"]");
                List<string> files = new List<string> ();
                List<string> directories = new List<string> ();
                foreach (HtmlNode node in collection) {
                    string href = node.GetAttributeValue ("href", null);
                    string title = node.GetAttributeValue ("title", null);
                    if (!string.IsNullOrEmpty (href) && !string.IsNullOrEmpty (title) && !title.Equals ("Go to parent directory")) {
                        Match match = Regex.Match (href, @"^/[^/]+/[^/]+/(tree|blob)/master/(?:.+/)*(.+)$");
                        if (match.Groups[1].ToString ().Equals ("tree"))
                            directories.Add (match.Groups[2].ToString ());
                        else
                            files.Add (match.Groups[2].ToString ());
                    }
                }
                string properUrl = AdjustUrl (url);
                UrlHandleFiles (files, properUrl, false, null);
                UrlHandleFolders (directories, properUrl);
            }
        }

        private void UrlHandleFiles (List<string> files, string url, bool fromDirectory, string dirname) {
            foreach (string filename in files) {
                if (!ignored.Contains (filename) && Regex.IsMatch (filename, @"^[A-Za-z\d]+\.[A-Za-z\d]+$")) {
                    int pos = filename.IndexOf ('.', StringComparison.Ordinal);
                    string id = filename.Substring (0, pos);
                    string ext = filename.Substring (pos + 1);
                    string fullUrl = url + filename;
                    if (!ignored.Contains (id) && !ignored.Contains ('.' + ext)) {
                        if (fromDirectory && (id.Equals ("main") || id.Equals (dirname)))
                            TryAdd (fullUrl, dirname, ext);
                        else if (!fromDirectory)
                            TryAdd (fullUrl, id, ext);
                    }
                }
            }
        }

        private void UrlHandleFolders (List<string> directory, string url) {
            foreach (string dirname in directory) {
                if (!ignored.Contains (dirname)) {
                    HtmlDocument doc = web.Load (url + dirname);
                    HtmlNodeCollection collection = doc.DocumentNode.SelectNodes ("//*[@class=\"js-navigation-open\"]");
                    List<string> files = new List<string> ();
                    foreach (HtmlNode node in collection) {
                        string href = node.GetAttributeValue ("href", null);
                        string title = node.GetAttributeValue ("title", null);
                        if (!string.IsNullOrEmpty (href) && !string.IsNullOrEmpty (title) && !title.Equals ("Go to parent directory")) {
                            Match match = Regex.Match (href, @"^/[^/]+/[^/]+/blob/master/(?:.+/)*(.+)$");
                            if (match.Groups[1].ToString ().Equals ("blob"))
                                files.Add (match.Groups[2].ToString ());
                        }
                    }
                    UrlHandleFiles (files, url + dirname + '/', true, dirname);
                }
            }
        }

        private void CheckFolders () {
            while (folders.Count > 0) {
                Folder folder = folders.Pop ();
                string url = AdjustUrl (folder.Url);
                Console.WriteLine (url);
                while (folder.Count > 0) {
                    string path = folder.Next ();
                    DirectoryInfo dir = new DirectoryInfo (path);
                    FolderHandleFiles (dir, url, false);
                    FolderHandleFolders (dir, url);
                }
            }
        }

        private void FolderHandleFiles (DirectoryInfo dir, string url, bool fromDirectory) {
            List<string> files = new List<string> ();
            foreach (FileInfo info in dir.GetFiles ())
                files.Add (info.Name);
            UrlHandleFiles (files, url, fromDirectory, null);
        }

        private void FolderHandleFolders (DirectoryInfo dir, string url) {
            foreach (DirectoryInfo info in dir.GetDirectories ())
                if (!ignored.Contains (info.Name))
                    FolderHandleFiles (info, url + info.Name + '/', true);
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
                    a.Stop ();
                    Console.Write (a.Elapsed.Milliseconds + " ");
                    HtmlNode node = doc.DocumentNode.SelectSingleNode ("//head/title");
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
    }
}