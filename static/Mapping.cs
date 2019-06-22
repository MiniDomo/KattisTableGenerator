using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KattisTableGenerator {
    public class Mapping {
        private const int DEFAULT_SIZE = 2500;
        private const string filename = "KattisMapping.txt";
        private static Dictionary<string, string> mappings = new Dictionary<string, string> (DEFAULT_SIZE);

        private Mapping () { }

        public static bool FileExists () {
            if (File.Exists (filename))
                return true;
            Console.WriteLine ("It is recommended to run KattisMapGenerator.dll to ensure fast runtimes.");
            return false;
        }

        public static void CreateFile () {
            File.Create (filename).Close ();
        }

        public static void AssignMappings () {
            string[] lines = File.ReadAllLines (filename, UnicodeEncoding.Default);
            // check if odd
            if ((lines.Length & 1) == 1) {
                string message = string.Format ("Found an odd amount of lines in {0}, indicating that a problem ID or problem name is missing. " +
                    "To fix this, try deleting {0} and running KattisMapGenerator.dll or manually edit {0}.", filename);
                throw new Exception (message);
            }
            for (int i = 0; i < lines.Length; i += 2) {
                string id = lines[i].Trim ();
                string name = lines[i + 1].Trim ();
                mappings.Add (id, name);
            }
        }

        public static bool ContainsKey (string key) {
            return mappings.ContainsKey (key);
        }

        public static string Get (string key) {
            return mappings[key];
        }
    }
}