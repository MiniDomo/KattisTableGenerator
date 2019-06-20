using System.Collections.Generic;

namespace KattisTableGenerator {
    public class Folder {
        public string Url { get; }
        public Queue<string> Paths { get; }

        public Folder (string Url) {
            this.Url = Url;
            Paths = new Queue<string> ();
        }

        public void AddPath (string path) {
            Paths.Enqueue (path);
        }

        public int Size () {
            return Paths.Count;
        }
    }
}