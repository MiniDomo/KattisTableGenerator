using System.Collections.Generic;
using System.IO;
using System.Json;

namespace KattisTableGenerator {
    public class ValidExtensions {
        private static Dictionary<string, string> extensions = new Dictionary<string, string> ();

        private ValidExtensions () { }
        public static void LoadExtensions () {
            string jsonString = File.ReadAllText ("extensions.json");
            JsonObject res = JsonObject.Parse (jsonString) as JsonObject;
            foreach (var pair in res) {
                string lang = pair.Key;
                JsonArray value = pair.Value as JsonArray;
                foreach (var ext in value) {
                    string formatted = ext.ToString ();
                    extensions.Add (formatted.Substring (1, formatted.Length - 2), lang);
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