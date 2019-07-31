using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using Wayfarer.Core.Utils.Files;
using Directory = System.IO.Directory;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

namespace Wayfarer.Core.Utils.Debug
{
    public static class Log
    {
        private static LoggingLevel _loggingLevel = LoggingLevel.All;
        public static LoggingLevel LoggingLevel => _loggingLevel;
        
        private static DirectoryInfo _logDir;
        private static Wayfarer _wayfarer;
        
        public static Wayfarer Wayfarer => _wayfarer;
        public static Wayfarer Wf => _wayfarer;
        
        private static FileInfo _logPrint;
        private static FileInfo _logError;
        private static FileInfo _logCrash;
        private static FileInfo _logNetwork;
        private static FileInfo _logUi;
        private static FileInfo _logDb;
        private static FileInfo _logConsole;
        private static FileInfo _logServer;
        
        private static Queue<PrintJob> _printQueue = new Queue<PrintJob>();

        private static Stopwatch _sw;
        private static Timer _timer;

        private static bool _initialized = false;

        internal static Queue<PrintJob> PrintQueue => _printQueue;
        internal static Stopwatch Stopwatch => _sw;

        static Log()
        {
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Couldn't initialize Log (static)");
                
                Initialize();
            }
        }
        
        public static void Initialize()
        {
            if (!_initialized)
            {
                _logDir = Directory.CreateDirectory(Paths.LogPath);

                FileInfo[] files = _logDir.GetFiles();
                foreach (FileInfo fi in files)
                {
                    if (      fi.Name == "___event.log"
                           || fi.Name == "__error.log"
                           || fi.Name == "_crash.log"
                           || fi.Name == "network.log"
                           || fi.Name == "ui.log"
                           || fi.Name == "database.log"
                           || fi.Name == "console.log"
                           || fi.Name == "server.log")
                    {
                        fi.Delete();
                    }
                }
                
                _logPrint = new FileInfo(Path.Combine(Paths.LogPath, "___event.log"));
                _logError = new FileInfo(Path.Combine(Paths.LogPath, "__error.log"));
                _logCrash = new FileInfo(Path.Combine(Paths.LogPath, "_crash.log"));
                _logNetwork = new FileInfo(Path.Combine(Paths.LogPath, "network.log"));
                _logUi = new FileInfo(Path.Combine(Paths.LogPath, "ui.log"));
                _logDb = new FileInfo(Path.Combine(Paths.LogPath, "database.log"));
                _logConsole = new FileInfo(Path.Combine(Paths.LogPath, "console.log"));
                _logServer = new FileInfo(Path.Combine(Paths.LogPath, "server.log"));
    
                _logPrint.Create().Dispose();
                _logError.Create().Dispose();
                _logCrash.Create().Dispose();
                _logNetwork.Create().Dispose();
                _logUi.Create().Dispose();
                _logDb.Create().Dispose();
                _logConsole.Create().Dispose();
                _logServer.Create().Dispose();
                _sw = new Stopwatch();
                _timer = new Timer();
                _sw.Start();
    
                // TODO: Consider creating a more sophisticated iterator for the Database methods
                _timer.Interval = 100;
                _timer.Elapsed += delegate { ProcessQueue(); };
                _timer.Enabled = true;

                _wayfarer = new Wayfarer();
                
                _initialized = true;
            }
        }

        public static void Immediate(string value)
        {
            // TODO: This will be used in Exceptions and catch statements
            // We want this to immediately write the last line before crash
        }
        
        public static void Simple(string value, bool gdPrint = false)
        {
            string print = value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public static void Print(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));

            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public static void Print(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.Message));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, "in: " + e.Source));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.StackTrace));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(print);
                GD.Print("");
                GD.Print(e.Message);
                GD.Print("in: " + e.Source);
                GD.Print(e.StackTrace);
                GD.Print("");
            }
        }
        
        public static void Error(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string error = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, error));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public static void Error(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string error = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.Message));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, "in: " + e.Source));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.StackTrace));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            
            _printQueue.Enqueue(new PrintJob(_logError.FullName, error));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, e.Message));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, "in: " + e.Source));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, e.StackTrace));
            _printQueue.Enqueue(new PrintJob(_logError.FullName, ""));
            
            
            if (gdPrint)
            {
                GD.Print(print);
                GD.Print("");
                GD.Print(e.Message);
                GD.Print("in: " + e.Source);
                GD.Print(e.StackTrace);
                GD.Print("");
            }
        }
        
        public static void Crash(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(alert);
                GD.Print(print);
                GD.Print(alert);
            }
            
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, crash, true)); // we cause an exception with the last bool parameter here
        }
        
        public static void Crash(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, crash));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, e.Message));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, "in: " + e.Source));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, e.StackTrace));
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, ""));
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.Message));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, "in: " + e.Source));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, e.StackTrace));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(print);
                GD.Print("");
                GD.Print(e.Message);
                GD.Print("in: " + e.Source);
                GD.Print(e.StackTrace);
                GD.Print("");
            }
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, ""));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert, true)); // we cause an exception with the last bool parameter here
        }
        
        public static void Ui(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string ui = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "UI: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logUi.FullName, ui));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public static void Database(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string db = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "DATABASE: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logDb.FullName, db));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public static void Console(string value, bool gdPrint = false)
        {
            string console = value;
            string print = _sw.ElapsedMilliseconds + " | " + "CONSOLE: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logConsole.FullName, console));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        internal static string ParseFilePathToTypeName(string fullPath) // NOTE: This may not be very performant, it's a quick hack, we'll figure a better way later
        {
            string[] split = fullPath.Split("\\");
            string last = split[split.Length - 1];
            char[] lasts = last.ToCharArray();
            char[] final = new char[lasts.Length - 3];
            for (int i = 0; i < final.Length; i++)
            {
                final[i] = lasts[i];
            }

            string finalString = "";
            foreach (char c in final)
            {
                finalString = String.Concat(finalString, c);
            }

            return finalString;
        }
        
        internal static void ProcessQueue()
        {
            while (_printQueue.Count > 0)
            {
                PrintJob job = _printQueue.Dequeue();
                using (StreamWriter writer = new StreamWriter(job.LogPath, true))
                {
                    writer.WriteLine(job.Print);
                    writer.NewLine = "";
                    if (job.Crash || job.Exception != null)
                    {
                        if (job.Crash && job.Exception == null)
                        {
                            throw new WayfarerException("Log.ProcessQueue() Demanded that we crash (above message)");
                        }
                        else
                        {
                            throw job.Exception;
                        }
                        
                    }
                    writer.Dispose();
                }
            }
        }

        internal struct PrintJob
        {
            public readonly string LogPath;
            public readonly string Print;
            public readonly bool Crash;
            public readonly Exception Exception;
            
            public PrintJob(string logPath, string print, bool crash = false)
            {
                LogPath = logPath;
                Print = print;
                Crash = crash;
                Exception = null;
            }

            public PrintJob(string logPath, string print, Exception e)
            {
                LogPath = logPath;
                Print = print;
                Crash = false;
                Exception = e;
            }
        }
    }
}