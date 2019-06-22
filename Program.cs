using System;
using System.IO;

namespace KattisTableGenerator {
    public class Program {
        public static void Main (string[] args) {
            if (!Mapping.FileExists ())
                Mapping.CreateFile ();
            Mapping.AssignMappings ();
            Config config = new Config ().Load ();
            Generator generator = new Generator (config);
            Console.WriteLine (config + "\n");
            generator.ProcessConfig ();
            using (StreamWriter file = new StreamWriter ("README.md")) {
                file.WriteLine ("# Kattis Solutions");
                file.WriteLine ("Some solutions may be outdated and could be revised.");
                string table = generator.GetTableString ();
                file.WriteLine (table);
            }
        }
    }
}
/* 
get id and name mappings
read config
check folders
check urls
 */