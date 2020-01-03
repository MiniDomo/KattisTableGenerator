using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace KattisTableGenerator {
    public class Mapping {
        private const int DEFAULT_SIZE = 2500;
        private const string fileName = "KattisMapping.txt";
        private static SortedList<string, string> mappings = new SortedList<string, string> (DEFAULT_SIZE);

        private Mapping () { }

        public static bool FileExists () {
            Logger.WriteLine ($"Looking for {fileName}...");
            if (File.Exists (fileName)) {
                Logger.WriteLine ("File found!");
                return true;
            }
            Logger.WriteLine ("File not found. It is recommended to run `dotnet KattisTableGenerator.dll --map` to ensure fast runtimes.");
            return false;
        }

        public static void CreateFile () {
            Logger.WriteLine ($"Creating an empty {fileName}.");
            File.Create (fileName).Close ();
        }

        public static void AssignMappings () {
            Logger.WriteLine ("Assigning Kattis IDs and Names...");
            string[] lines = File.ReadAllLines (fileName, UnicodeEncoding.Default);
            // check if odd
            if (lines.Length == 0)
                Logger.WriteLine ("It is recommended to run `dotnet KattisTableGenerator.dll --map` to ensure fast runtimes.");
            if ((lines.Length & 1) == 1) {
                string message = string.Format ("Found an odd amount of lines in {0}, indicating that a problem ID or problem name is missing. " +
                    "To fix this, try deleting {0} and running `dotnet KattisTableGenerator.dll --map` or manually edit {0}.", fileName);
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
                if (collection != null) {
                    foreach (HtmlNode node in collection) {
                        string name = WebUtility.HtmlDecode (node.InnerText.Trim ());
                        string href = node.FirstChild.GetAttributeValue ("href", string.Empty);
                        string id = href.Substring (href.LastIndexOf ("/", StringComparison.Ordinal) + 1);
                        Logger.WriteLine ($"Added problem {name} ({id})");
                        if (!mappings.ContainsKey (id))
                            mappings.Add (id, name);
                    }
                    HtmlNode nextButton = doc.DocumentNode.SelectSingleNode ("//*[@id=\"problem_list_paginate\"]")?.LastChild;
                    if (nextButton != null) {
                        string classValue = nextButton.GetAttributeValue ("class", string.Empty);
                        if (classValue.Equals ("enabled")) {
                            url = $"https://open.kattis.com/problems?page={i++}&order=name";
                            done = false;
                        }
                    }
                } else {
                    Logger.WriteLine ($"Could not find problems: {url}");
                }
            }
            Logger.WriteLine ("Finished updating map.");
            Logger.WriteLine ($"Map now has {mappings.Count} problem(s)");
        }

        public static void UpdateFile () {
            Logger.WriteLine ($"Updating {fileName}.");
            int STRINGBUILDER_SIZE = 100000;
            StringBuilder builder = new StringBuilder (STRINGBUILDER_SIZE);
            foreach (var pair in mappings)
                builder.Append (pair.Key).Append ('\n').Append (pair.Value).Append ('\n');
            File.WriteAllText (fileName, builder.ToString ().TrimEnd ());
            Logger.WriteLine ($"Updated {fileName}.");
        }
    }
}