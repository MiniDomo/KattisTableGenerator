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
            Logger.WriteLine ("Reading {0}...", filename);
            string[] lines = File.ReadAllLines (filename, UnicodeEncoding.Default);
            FileState state = FileState.NONE;
            foreach (string original in lines) {
                string line = original.Trim ();
                if (line.Length != 0 && !IsFileState (line, ref state)) {
                    if (state == FileState.IGNORE && !Ignored.Contains (line)) {
                        Logger.WriteLine ("Added {0} to ignored list.", line);
                        Ignored.Add (line);
                    } else if (state == FileState.URL) {
                        if (Urls.Contains (line))
                            Logger.WriteLine ("Duplicate Url found: {0}", line);
                        else if (!IsProperFormatGithubUrl (line))
                            Logger.WriteLine ("Invalid Url found: {0}", line);
                        else {
                            Logger.WriteLine ("Added {0} to Urls.", line);
                            Urls.Add (line);
                        }
                    } else if (state == FileState.FOLDER) {
                        if (line.StartsWith ("to:", StringComparison.OrdinalIgnoreCase)) {
                            if (line.Length > 3) {
                                string url = line.Substring (3);
                                if (IsProperFormatGithubUrl (url)) {
                                    Logger.WriteLine ("Added new folder with url {0}.", url);
                                    Folders.Push (new Folder (url));
                                } else
                                    Logger.WriteLine ("Invalid Url for FOLDER: {0}", url);
                            } else
                                Logger.WriteLine ("Invalid line for FOLDER: {0}", line);
                        } else {
                            if (Folders.Count > 0) {
                                Logger.WriteLine ("Attached {0} to previous url.", line);
                                Folders.Peek ().Add (line);
                            } else
                                Logger.WriteLine ("No target Url for FOLDER found when attempting to add {0}.", line);
                        }
                    }
                }
            }
            Logger.WriteLine ("Finished reading {0}.", filename);
            return this;
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
            string res = "";
            if (Ignored.Count > 0) {
                res += "IGNORE\n";
                foreach (string ignored in Ignored)
                    res += ignored + '\n';
                res += '\n';
            }
            if (Urls.Count > 0) {
                res += "URL\n";
                foreach (string url in Urls)
                    res += url + '\n';
                res += '\n';
            }
            if (Folders.Count > 0) {
                res += "FOLDER\n";
                foreach (Folder folder in Folders) {
                    res += "TO:" + folder.Url + '\n';
                    foreach (string paths in folder.Paths)
                        res += paths + '\n';
                }
            }
            res = res.Trim ();
            return res;
        }
    }
}