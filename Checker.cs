using System.Collections.Generic;

namespace KattisTableGenerator {
    public class Checker {
        private HashSet<string> ignored, urls;
        private Stack<Folder> folders;
        public Checker (Config config) {
            folders = config.Folders;
            ignored = config.Ignored;
            urls = config.Urls;
        }
    }
}