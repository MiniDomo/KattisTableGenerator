using System;

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
            // KattisProblem a = new KattisProblem ("name", "id");
            // a.Add ("C++", "google.com");
            // Console.WriteLine (a.Contains ("C++"));
            // Console.WriteLine (a);
            // a.Add ("Jaa", "kattis.com");
            // Console.WriteLine (a);

        }
    }
}
/* 
get id and name mappings
read config
check folders
check urls
 */