using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KattisTableGenerator {
    public class ValidExtensions {
        private static Dictionary<string, string> extensions = new Dictionary<string, string> ();

        private ValidExtensions () { }
        public static void LoadExtensions () {
            string jsonString = File.ReadAllText ("extensions.json");
            Language[] languages = JsonSerializer.Deserialize<Language[]> (jsonString);
            foreach (var lang in languages) {
                foreach (var ext in lang.Extensions) {
                    extensions.Add (ext, lang.Name);
                }
            }
        }

        public static bool Contains (string extension) {
            return extensions.ContainsKey (extension);
        }

        public static string Get (string extension) {
            return extensions[extension];
        }
    }
}