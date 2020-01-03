using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace KattisTableGenerator {
    public class Config {
        private const string fileName = "config.json";
        public Configuration configuration;
        public HashSet<string> Urls { get; }
        public HashSet<string> Ignored { get; }
        public Stack<Folder> Folders { get; }

        public Config () {
            Logger.WriteLine ($"Reading {fileName}...");
            if (!File.Exists (fileName)) {
                Logger.WriteLine ("{0} not found. Creating {0}.", fileName);
                configuration = new Configuration ();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize (configuration, options);
                File.WriteAllText (fileName, jsonString);
            } else {
                string jsonString = File.ReadAllText (fileName);
                configuration = JsonSerializer.Deserialize<Configuration> (jsonString);
            }
            Logger.WriteLine ($"Finished reading {fileName}.");
            Urls = new HashSet<string> ();
            Ignored = new HashSet<string> ();
            Folders = new Stack<Folder> ();
            AddUrls ();
            AddIgnored ();
            AddFolders ();

        }

        private void AddFolders () {
            foreach (Folder folder in configuration.Folders) {
                if (IsProperFormatGithubUrl (folder.BaseUrl)) {
                    Folders.Push (folder);
                    Logger.WriteLine ($"Added new folder with url {folder.BaseUrl}.");
                } else {
                    Logger.WriteLine ($"Invalid Url for FOLDER: {folder.BaseUrl}");
                }
            }
        }

        private void AddIgnored () {
            foreach (string ignore in configuration.Ignore) {
                if (!Ignored.Contains (ignore)) {
                    Logger.WriteLine ($"Added {ignore} to ignored list.");
                    Ignored.Add (ignore);
                }
            }
        }

        private void AddUrls () {
            foreach (string url in configuration.Url) {
                if (Urls.Contains (url))
                    Logger.WriteLine ($"Duplicate Url found: {url}");
                else if (!IsProperFormatGithubUrl (url))
                    Logger.WriteLine ($"Invalid Url found: {url}");
                else {
                    Logger.WriteLine ($"Added {url} to Urls.");
                    Urls.Add (url);
                }
            }
        }

        private bool IsProperFormatGithubUrl (string url) {
            return Uri.IsWellFormedUriString (url, UriKind.Absolute) && Regex.IsMatch (url, @"^https://github.com/[^/]+/[^/]+(/|/tree/master/([^/]+/?)+)?$");
        }

        public override string ToString () {
            int DEFAULT_SIZE = 10000;
            StringBuilder builder = new StringBuilder (DEFAULT_SIZE);
            if (Ignored.Count > 0) {
                builder.Append ("IGNORE").AppendLine ();
                foreach (string ignored in Ignored)
                    builder.Append (ignored).AppendLine ();
                builder.AppendLine ();
            }
            if (Urls.Count > 0) {
                builder.Append ("URL").AppendLine ();
                foreach (string url in Urls)
                    builder.Append (url).AppendLine ();
                builder.AppendLine ();
            }
            if (Folders.Count > 0) {
                builder.Append ("FOLDER").AppendLine ();
                foreach (Folder folder in Folders) {
                    builder.Append ("TO:").Append (folder.BaseUrl).AppendLine ();
                    foreach (string paths in folder.Paths)
                        builder.Append (paths).AppendLine ();
                }
            }
            return builder.ToString ().TrimEnd ();
        }
    }
}