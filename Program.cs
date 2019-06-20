using System;

namespace KattisTableGenerator {
    public class Program {
        public static void Main (string[] args) {
            if (!Mapping.FileExists ())
                Mapping.CreateFile ();
            Mapping.AssignMappings ();
            Config config = new Config ().Load ();
            Checker checker = new Checker (config);
            Console.WriteLine (config.GetLayout ());
        }
    }
}
/* 
get id and name mappings
read config
check folders
check urls
 */