using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KattisTableGenerator {
    public class Config {
        private const string filename = "Config.txt";
        public HashSet<string> Urls { get; }
        public HashSet<string> Ignored { get; }
        public Stack<Folder> Folders { get; }

        public Config () {
            if (!File.Exists (filename)) {
                Logger.WriteLine ("{0} not found. Creating {0}.", filename);
                File.Create (filename).Close ();
            }
            Urls = new HashSet<string> ();
            Ignored = new HashSet<string> ();
            Folders = new Stack<Folder> ();
        }

        public Config Load () {
            Logger.WriteLine ($"Reading {filename}...");
            string[] lines = File.ReadAllLines (filename, UnicodeEncoding.Default);
            FileState state = FileState.NONE;
            foreach (string original in lines) {
                string line = original.Trim ();
                if (!string.IsNullOrEmpty (line) && !line.StartsWith ("#", StringComparison.OrdinalIgnoreCase) && !IsFileState (line, ref state)) {
                    if (state == FileState.IGNORE) {
                        HandleIgnore (line);
                    } else if (state == FileState.URL) {
                        HandleUrl (line);
                    } else if (state == FileState.FOLDER) {
                        HandleFolder (line);
                    }
                }
            }
            Logger.WriteLine ($"Finished reading {filename}.");
            return this;
        }

        private void HandleIgnore (string line) {
            if (!Ignored.Contains (line)) {
                Logger.WriteLine ($"Added {line} to ignored list.");
                Ignored.Add (line);
            }
        }

        private void HandleUrl (string line) {
            if (Urls.Contains (line))
                Logger.WriteLine ($"Duplicate Url found: {line}");
            else if (!IsProperFormatGithubUrl (line))
                Logger.WriteLine ($"Invalid Url found: {line}");
            else {
                Logger.WriteLine ($"Added {line} to Urls.");
                Urls.Add (line);
            }
        }

        private void HandleFolder (string line) {
            if (line.StartsWith ("to:", StringComparison.OrdinalIgnoreCase)) {
                if (line.Length > 3) {
                    string url = line.Substring (3);
                    if (IsProperFormatGithubUrl (url)) {
                        Logger.WriteLine ($"Added new folder with url {url}.");
                        Folders.Push (new Folder (url));
                    } else
                        Logger.WriteLine ($"Invalid Url for FOLDER: {url}");
                } else
                    Logger.WriteLine ($"Invalid line for FOLDER: {line}");
            } else {
                if (Folders.Count > 0) {
                    Logger.WriteLine ($"Attached {line} to previous url.");
                    Folders.Peek ().Add (line);
                } else
                    Logger.WriteLine ($"No target Url for FOLDER found when attempting to add {line}.");
            }
        }

        private bool IsProperFormatGithubUrl (string url) {
            return Uri.IsWellFormedUriString (url, UriKind.Absolute) && Regex.IsMatch (url, @"^https://github.com/[^/]+/[^/]+(/|/tree/master/([^/]+/?)+)?$");
        }

        private bool IsFileState (string line, ref FileState state) {
            if (string.Equals (line, "ignore", StringComparison.OrdinalIgnoreCase))
                state = FileState.IGNORE;
            else if (string.Equals (line, "url", StringComparison.OrdinalIgnoreCase))
                state = FileState.URL;
            else if (string.Equals (line, "folder", StringComparison.OrdinalIgnoreCase))
                state = FileState.FOLDER;
            else
                return false;
            return true;
        }

        private enum FileState {
            IGNORE,
            URL,
            FOLDER,
            NONE
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
                    builder.Append ("TO:").Append (folder.Url).AppendLine ();
                    foreach (string paths in folder.Paths)
                        builder.Append (paths).AppendLine ();
                }
            }
            return builder.ToString ().TrimEnd ();
        }
    }
}