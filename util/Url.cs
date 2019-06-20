using System;
using System.Text.RegularExpressions;

namespace KattisTableGenerator {
    public class Url {
        public static Regex githubFolder = new Regex (@"^https://github.com/[^/]+/[^/]+(/|/tree/master/([^/]+/?)+)?$", RegexOptions.Compiled);

        public static bool IsProperFormatGithubUrl (string url) {
            return Uri.IsWellFormedUriString (url, UriKind.Absolute) && githubFolder.IsMatch (url);
        }
    }
}