using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace KattisTableGenerator {
    public class Mapping {
        private const int DEFAULT_SIZE = 2500;
        private const string filename = "KattisMapping.txt";
        private static SortedList<string, string> mappings = new SortedList<string, string> (DEFAULT_SIZE);

        private Mapping () { }

        public static bool FileExists () {
            Logger.WriteLine ("Looking for {0}...", filename);
            if (File.Exists (filename)) {
                Logger.WriteLine ("File found!");
                return true;
            }
            Logger.WriteLine ("File not found. It is recommended to run `dotnet KattisTableGenerator.dll map` to ensure fast runtimes.");
            return false;
        }

        public static void CreateFile () {
            Logger.WriteLine ("Creating an empty {0}.", filename);
            File.Create (filename).Close ();
        }

        public static void AssignMappings () {
            Logger.WriteLine ("Assigning Kattis IDs and Names...");
            string[] lines = File.ReadAllLines (filename, UnicodeEncoding.Default);
            // check if odd
            if ((lines.Length & 1) == 1) {
                string message = string.Format ("Found an odd amount of lines in {0}, indicating that a problem ID or problem name is missing. " +
                    "To fix this, try deleting {0} and running `dotnet KattisTableGenerator.dll map` or manually edit {0}.", filename);
                throw new Exception (message);
            }
            for (int i = 0; i < lines.Length; i += 2) {
                string id = lines[i].Trim ();
                string name = lines[i + 1].Trim ();
                mappings.Add (id, name);
            }
            Logger.WriteLine ("Assignment done.");
        }

        public static bool ContainsKey (string key) {
            return mappings.ContainsKey (key);
        }

        public static string Get (string key) {
            return mappings[key];
        }

        public static void UpdateMap () {
            Logger.WriteLine ("Updating map...");
            HtmlWeb web = new HtmlWeb ();
            string url = "https://open.kattis.com/problems?page=0&order=name";
            int i = 1;
            bool done = false;
            while (!done) {
                done = true;
                HtmlDocument doc = web.Load (url);
                HtmlNodeCollection collection = doc.DocumentNode.SelectNodes ("//*[@class=\"name_column\"]");
                foreach (HtmlNode node in collection) {
                    string name = WebUtility.HtmlDecode (node.InnerText.Trim ());
                    string href = node.FirstChild.GetAttributeValue ("href", string.Empty);
                    string id = href.Substring (href.LastIndexOf ("/", StringComparison.Ordinal) + 1);
                    Logger.WriteLine ("Added problem {0} ({1})", id, name);
                    mappings.Add (id, name);
                }
                HtmlNode button = doc.DocumentNode.SelectSingleNode ("//*[@id=\"problem_list_paginate\"]").LastChild;
                string classValue = button.GetAttributeValue ("class", string.Empty);
                if (classValue.Equals ("enabled")) {
                    url = string.Format ("https://open.kattis.com/problems?page={0}&order=name", i++);
                    done = false;
                }
            }
            Logger.WriteLine ("Finished updating map.");
            Logger.WriteLine ("Map now has {0} problem(s)", mappings.Count);
        }

        public static void UpdateFile () {
            Logger.WriteLine ("Updating {0}.", filename);
            StringBuilder builder = new StringBuilder ();
            foreach (var pair in mappings)
                builder.Append (pair.Key).Append ('\n').Append (pair.Value).Append ('\n');
            File.WriteAllText (filename, builder.ToString ().TrimEnd ());
            Logger.WriteLine ("Updated {0}.", filename);
        }
    }
}