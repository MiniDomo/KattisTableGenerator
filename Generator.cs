using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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
            int DEFAULT_SIZE = 100000;
            StringBuilder builder = new StringBuilder (DEFAULT_SIZE).AppendFormat ("| {0} Problem{1} | Languages |\n| - | - |\n", list.Count, list.Count == 1 ? "" : "s");
            foreach (KattisProblem problem in list)
                builder.Append (problem).AppendLine ();
            return builder.ToString ().TrimEnd ();
        }

        public void ProcessConfig () {
            Logger.WriteLine ("Checking category FOLDER...");
            CheckFolders ();
            Logger.WriteLine ("Finished checking category FOLDER.");
            Logger.WriteLine ("Checking category URL...");
            CheckUrls ();
            Logger.WriteLine ("Finished checking category URL.");
        }

        private void CheckUrls () {
            foreach (string url in urls) {
                HtmlDocument doc = web.Load (url);
                HtmlNodeCollection collection = doc.DocumentNode.SelectNodes ("//*[@class=\"js-navigation-open\"]");
                if (collection != null) {
                    List<string> files = new List<string> ();
                    List<string> directories = new List<string> ();
                    foreach (HtmlNode node in collection) {
                        string href = node.GetAttributeValue ("href", string.Empty);
                        string title = node.GetAttributeValue ("title", string.Empty);
                        if (!string.IsNullOrEmpty (href) && !string.IsNullOrEmpty (title) && !title.Equals ("Go to parent directory")) {
                            Match match = Regex.Match (href, @"^/[^/]+/[^/]+/(tree|blob)/master/(?:.+/)*(.+)$");
                            string name = match.Groups[2].ToString ();
                            if (match.Groups[1].ToString ().Equals ("tree"))
                                directories.Add (name);
                            else
                                files.Add (name);
                        }
                    }
                    string properUrl = AdjustUrl (url);
                    HandleFiles (files, properUrl, false, null);
                    UrlHandleDirectories (directories, properUrl);
                } else
                    Logger.WriteLine ($"Could not find folders or files: {url}");
            }
        }

        private void HandleFiles (List<string> files, string url, bool fromDirectory, string dirname) {
            foreach (string filename in files) {
                if (!ignored.Contains (filename)) {
                    if (Regex.IsMatch (filename, @"^[A-Za-z\d]+\.[A-Za-z\d]+$")) {
                        int pos = filename.IndexOf ('.', StringComparison.Ordinal);
                        string id = filename.Substring (0, pos);
                        string ext = filename.Substring (pos + 1);
                        string fullUrl = url + filename;
                        if (!ignored.Contains (id) && !ignored.Contains ('.' + ext)) {
                            if (fromDirectory && (id.Equals ("main") || id.Equals (dirname)))
                                TryAdd (fullUrl, dirname, ext);
                            else if (!fromDirectory)
                                TryAdd (fullUrl, id, ext);
                        } else
                            Logger.WriteLine ($"Ignored file: {filename}");
                    } else
                        Logger.WriteLine ($"Invalid filename formatting: {filename}");
                } else
                    Logger.WriteLine ($"Ignored file: {filename}");
            }
        }

        private void UrlHandleDirectories (List<string> directory, string url) {
            foreach (string dirname in directory) {
                if (!ignored.Contains (dirname)) {
                    HtmlDocument doc = web.Load (url + dirname);
                    HtmlNodeCollection collection = doc.DocumentNode.SelectNodes ("//*[@class=\"js-navigation-open\"]");
                    if (collection != null) {
                        List<string> files = new List<string> ();
                        foreach (HtmlNode node in collection) {
                            string href = node.GetAttributeValue ("href", string.Empty);
                            string title = node.GetAttributeValue ("title", string.Empty);
                            if (!string.IsNullOrEmpty (href) && !string.IsNullOrEmpty (title) && !title.Equals ("Go to parent directory")) {
                                Match match = Regex.Match (href, @"^/[^/]+/[^/]+/blob/master/(?:.+/)*(.+)$");
                                if (match.Success)
                                    files.Add (match.Groups[1].ToString ());
                            }
                        }
                        HandleFiles (files, url + dirname + '/', true, dirname);
                    } else
                        Logger.WriteLine ($"Could not find folders or files: {url + dirname}");
                } else
                    Logger.WriteLine ($"Ignored directory: {dirname}");
            }
        }

        private void CheckFolders () {
            while (folders.Count > 0) {
                Folder folder = folders.Pop ();
                string url = AdjustUrl (folder.Url);
                while (folder.Count > 0) {
                    string path = folder.Next ();
                    DirectoryInfo dir = new DirectoryInfo (path);
                    FolderHandleFiles (dir, url, false);
                    FolderHandleDirectories (dir, url);
                }
            }
        }

        private void FolderHandleFiles (DirectoryInfo dir, string url, bool fromDirectory) {
            List<string> files = new List<string> ();
            foreach (FileInfo info in dir.GetFiles ())
                files.Add (info.Name);
            HandleFiles (files, url, fromDirectory, fromDirectory ? dir.Name : null);
        }

        private void FolderHandleDirectories (DirectoryInfo dir, string url) {
            foreach (DirectoryInfo info in dir.GetDirectories ())
                if (!ignored.Contains (info.Name))
                    FolderHandleFiles (info, url + info.Name + '/', true);
                else
                    Logger.WriteLine ($"Ignored directory: {info.Name}");
        }

        private void TryAdd (string url, string id, string ext) {
            if (!ValidExtensions.Contains (ext)) {
                Logger.WriteLine ($"Unknown extension found: {ext}");
                return;
            }
            if (table.ContainsKey (id)) {
                KattisProblem problem = table[id];
                string lang = ValidExtensions.Get (ext);
                if (!problem.Contains (lang)) {
                    Logger.WriteLine ($"Added {lang} to problem {id}");
                    problem.Add (lang, url);
                } else
                    Logger.WriteLine ($"{lang} already found in problem {id}");
            } else {
                string name = string.Empty;
                if (Mapping.ContainsKey (id)) {
                    name = Mapping.Get (id);
                } else {
                    HtmlDocument doc = web.Load ($"https://open.kattis.com/problems/{id}");
                    HtmlNode node = doc.DocumentNode.SelectSingleNode ("//head/title");
                    if (node != null) {
                        Match match = Regex.Match (node.InnerHtml, @"(.+) &ndash; Kattis, Kattis");
                        name = WebUtility.HtmlDecode (match.Groups[1].ToString ());
                    } else
                        Logger.WriteLine ($"Could not find title: https://open.kattis.com/problems/{id}");
                }
                if (!string.IsNullOrEmpty (name) && !name.Equals ("404: Not Found")) {
                    table.Add (id, new KattisProblem (name, id));
                    KattisProblem problem = table[id];
                    string lang = ValidExtensions.Get (ext);
                    if (!problem.Contains (lang)) {
                        Logger.WriteLine ($"Added {lang} to problem {id}");
                        problem.Add (lang, url);
                    } else
                        Logger.WriteLine ($"{lang} already found in problem {id}");
                } else
                    Logger.WriteLine ($"Invalid name found with: {url} {id} {ext}");
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