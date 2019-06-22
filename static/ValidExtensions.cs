using System.Collections.Generic;

namespace KattisTableGenerator {
    public class ValidExtensions {
        private static Dictionary<string, string> ext = new Dictionary<string, string> { { ".c", "C" },
            { "h", "C" },
            { "cs", "C#" },
            { "C", "C++" },
            { "cc", "C++" },
            { "cpp", "C++" },
            { "cxx", "C++" },
            { "c++", "C++" },
            { "hh", "C++" },
            { "hpp", "C++" },
            { "hxx", "C++" },
            { "h++", "C++" },
            { "go", "Go" },
            { "hs", "Haskell" },
            { "lhs", "Haskell" },
            { "java", "Java" },
            { "js", "JavaScript" },
            { "kt", "Kotlin" },
            { "kts", "Kotlin" },
            { "m", "Objective-C" },
            { "mm", "Objective-C" },
            { "M", "Objective-C" },
            { "pp", "Pascal" },
            { "pas", "Pascal" },
            { "inc", "Pascal" },
            { "php", "PHP" },
            { "phtml", "PHP" },
            { "php3", "PHP" },
            { "php4", "PHP" },
            { "php5", "PHP" },
            { "php7", "PHP" },
            { "phps", "PHP" },
            { "php-s", "PHP" },
            { "pl", "Prolog" },
            { "pro", "Prolog" },
            { "P", "Prolog" },
            { "py", "Python" },
            { "pyc", "Python" },
            { "pyd", "Python" },
            { "pyo", "Python" },
            { "pyw", "Python" },
            { "pyz", "Python" },
            { "rb", "Ruby" },
            { "scala", "Scala" }
        };

        private ValidExtensions () { }

        public static bool Contains (string extension) {
            return ext.ContainsKey (extension);
        }

        public static string Get (string extension) {
            return ext[extension];
        }
    }
}