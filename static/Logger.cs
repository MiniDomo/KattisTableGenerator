using System;
using System.Diagnostics;

namespace KattisTableGenerator {
    public class Logger {
        private static Stopwatch watch = new Stopwatch ();

        private Logger () { }

        public static void WriteLine (object obj) {
            Console.WriteLine ("{0} {1}", GetTimestamp (), obj);
        }

        public static void WriteLine (string message) {
            Console.WriteLine ("{0} {1}", GetTimestamp (), message);
        }

        public static void WriteLine (string format, params object[] args) {
            Console.WriteLine (GetTimestamp () + " " + format, args);
        }

        private static string GetTimestamp () => string.Format ("[{0}]", watch.Elapsed);

        public static void Start () {
            watch.Start ();
        }

        public static void Stop () {
            watch.Stop ();
        }
    }
}