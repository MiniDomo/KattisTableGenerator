using System;
using System.IO;
using System.Text;

namespace KattisTableGenerator {
    public class Program {
        public static void Main (string[] args) {
            if (args.Length > 0) {
                if (string.Equals (args[0], "--map", StringComparison.OrdinalIgnoreCase)) {
                    Logger.Start ();
                    Logger.WriteLine ("Starting program...");
                    if (!Mapping.FileExists ())
                        Mapping.CreateFile ();
                    Mapping.UpdateMap ();
                    Mapping.UpdateFile ();
                    Logger.WriteLine ("Program finished.");
                    Logger.Stop ();
                } else {
                    Console.WriteLine ($"Unknown flag(s) found \"{string.Join (' ', args)}\". Use no flags to generate the Kattis table or use \"--map\" to update KattisMapping.txt.");
                }
            } else {
                ValidExtensions.LoadExtensions ();
                Logger.Start ();
                Logger.WriteLine ("Starting program...");
                if (!Mapping.FileExists ())
                    Mapping.CreateFile ();
                Mapping.AssignMappings ();
                Config config = new Config ();
                Generator generator = new Generator (config);
                generator.ProcessConfig ();
                using (StreamWriter file = new StreamWriter ("README.md", false, UnicodeEncoding.Default, 1 << 16)) {
                    Logger.WriteLine ("Writing to README.md");
                    file.WriteLine(config.GetBase());
                    file.WriteLine("\n");
                    file.WriteLine("## Kattis Solutions");
                    string table = generator.GetTableString ();
                    file.WriteLine (table);
                }
                Logger.WriteLine ("Program finished.");
                Logger.Stop ();
            }
        }
    }
}