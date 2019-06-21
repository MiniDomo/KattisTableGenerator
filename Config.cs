using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KattisTableGenerator {
    public class Config {
        private const string filename = "Config.txt";
        public HashSet<string> Urls { get; }
        public HashSet<string> Ignored { get; }
        public Stack<Folder> Folders { get; }

        public Config () {
            if (!File.Exists (filename)) {
                Console.WriteLine ("{0} not found. Creating {0}.", filename);
                File.Create (filename).Close ();
            }
            Urls = new HashSet<string> ();
            Ignored = new HashSet<string> ();
            Folders = new Stack<Folder> ();
        }

        public Config Load () {
            string[] lines = File.ReadAllLines (filename, UnicodeEncoding.Default);
            FileState state = FileState.NONE;
            foreach (string original in lines) {
                string line = original.Trim ();
                if (line.Length != 0 && !IsFileState (line, ref state)) {
                    if (state == FileState.IGNORE && !Ignored.Contains (line)) {
                        Ignored.Add (line);
                    } else if (state == FileState.URL && !Urls.Contains (line) && Url.IsProperFormatGithubUrl (line)) {
                        Urls.Add (line);
                    } else if (state == FileState.FOLDER) {
                        if (line.StartsWith ("to:", StringComparison.OrdinalIgnoreCase)) {
                            if (line.Length > 3) {
                                string url = line.Substring (3);
                                if (Url.IsProperFormatGithubUrl (url))
                                    Folders.Push (new Folder (url));
                            }
                        } else {
                            if (Folders.Count > 0)
                                Folders.Peek ().Add (line);
                        }
                    }
                }
            }
            return this;
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