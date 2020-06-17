using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KattisTableGenerator {
    public class Logger {
        private static Stopwatch watch = new Stopwatch ();
        private static StreamWriter stdout = new StreamWriter (Console.OpenStandardOutput (1 << 16), UnicodeEncoding.Default, 1 << 16);
        private const int DEFAULT_SIZE = 100000;
        private static StringBuilder builder = new StringBuilder (DEFAULT_SIZE);

        private Logger () { }

        public static void WriteLine (object obj) {
            string timestamp = GetTimestamp ();
            builder.Append (timestamp).Append (' ').Append (obj).AppendLine ();
            stdout.WriteLine ($"{timestamp} {obj}");
        }

        public static void WriteLine (string message) {
            string timestamp = GetTimestamp ();
            builder.Append (timestamp).Append (' ').Append (message).AppendLine ();
            stdout.WriteLine ($"{timestamp} {message}");
        }

        public static void WriteLine (string format, params object[] args) {
            string timestamp = GetTimestamp ();
            builder.Append (timestamp).Append (' ').AppendFormat (format, args).AppendLine ();
            stdout.WriteLine ($"{timestamp} {format}", args);
        }

        private static string GetTimestamp () => $"[{watch.Elapsed}]";

        public static void Start () {
            watch.Start ();
            stdout.AutoFlush = true;
        }

        public static void Stop () {
            watch.Stop ();
            stdout.Close ();
            if (!Directory.Exists (@".\logs\"))
                Directory.CreateDirectory (@".\logs\");
            using (StreamWriter file = new StreamWriter ($"logs\\{DateTime.Now.ToString("d-M-yyyy-hh-mm-ss")}.log", false, UnicodeEncoding.Default, 1 << 16)) {
                file.Write (builder);
            }
        }
    }
}