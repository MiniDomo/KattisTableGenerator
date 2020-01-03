namespace KattisTableGenerator {
    public class Configuration {
        public string[] Ignore { get; set; }
        public string[] Url { get; set; }
        public Folder[] Folders { get; set; }

        public Configuration () {
            Ignore = new string[0];
            Url = new string[0];
            Folders = new Folder[0];
        }
    }
}