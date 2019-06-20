using System;

namespace KattisTableGenerator {
    public class Program {
        public static void Main (string[] args) {
            if (!Mapping.FileExists ())
                Mapping.CreateFile ();
            Mapping.AssignMappings ();
            Config config = new Config ().Load ();
            Generator generator = new Generator (config);
            Console.WriteLine (config.GetValidConfig ());
            generator.checkFolders ();
            
        }
    }
}
/* 
get id and name mappings
read config
check folders
check urls
 */