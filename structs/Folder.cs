using System.Collections.Generic;

namespace KattisTableGenerator {
    public class Folder {
        public string Url { get; }
        public Queue<string> Paths { get; }

        public int Count { get => Paths.Count; }

        public Folder (string Url) {
            this.Url = Url;
            Paths = new Queue<string> ();
        }

        public void Add (string path) {
            Paths.Enqueue (path);
        }

        public string Next () {
            return Paths.Dequeue ();
        }
    }
}