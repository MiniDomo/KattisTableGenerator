using System;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace KattisTableGenerator {
    public class Program {
        public static void Main (string[] args) {
            if (!Mapping.FileExists ())
                Mapping.CreateFile ();
            Mapping.AssignMappings ();
            Config config = new Config ().Load ();
            Generator generator = new Generator (config);
            Console.WriteLine (config);
            Console.WriteLine ();
            generator.checkFolders ();
            Console.WriteLine (generator.GetTableString ());

            // KattisProblem a = new KattisProblem ("name", "id");
            // a.Add ("C++", "google.com");
            // Console.WriteLine (a.Contains ("C++"));
            // Console.WriteLine (a);
            // a.Add ("Jaa", "kattis.com");
            // Console.WriteLine (a);

            // string url = @"https://open.kattis.com/problems/redsocks";
            // HtmlWeb web = new HtmlWeb ();
            // HtmlDocument doc = web.Load (url);
            // HtmlNode node = doc.DocumentNode.SelectSingleNode ("//head/title");
            // Match match = Regex.Match (node.InnerHtml, @"(.+) &ndash; Kattis, Kattis");
            // Console.WriteLine (match.Groups[1]);
            // Console.WriteLine ();
        }
    }
}
/* 
get id and name mappings
read config
check folders
check urls
 */