﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using Wayfarer.Utils.Files;
using Directory = System.IO.Directory;
using Path = System.IO.Path;
using Timer = System.Timers.Timer;

namespace Wayfarer.Utils.Debug
{
    public static class Log
    {
        private static LoggingLevel _loggingLevel = LoggingLevel.All;
        public static LoggingLevel LoggingLevel => _loggingLevel;
        
        private static DirectoryInfo _logDir;
        private static DirectoryInfo _wayfarerDir;
        
        private static FileInfo _logPrint;
        private static FileInfo _logError;
        private static FileInfo _logCrash;
        private static FileInfo _logNetwork;
        private static FileInfo _logUi;
        private static FileInfo _logDb;
        private static FileInfo _logConsole;
        private static FileInfo _logServer;
        
        private static FileInfo _logWayfarerPrint;
        private static FileInfo _logWayfarerEditor;
        
        
        private static Queue<PrintJob> _printQueue = new Queue<PrintJob>();

        private static Stopwatch _sw;
        private static Timer _timer;

        private static bool _instantiated = false;

        static Log()
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

            _wayfarerDir = Directory.CreateDirectory(Paths.WayfarerLogPath);
            
            FileInfo[] wayfarerFiles = _wayfarerDir.GetFiles();
            foreach (FileInfo fi in wayfarerFiles)
            {
                if (fi.Name == "___event.log"
                    || fi.Name == "editor.log")
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
            
            _logWayfarerPrint = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "___event.log"));
            _logWayfarerEditor = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "editor.log"));

            _logPrint.Create().Dispose();
            _logError.Create().Dispose();
            _logCrash.Create().Dispose();
            _logNetwork.Create().Dispose();
            _logUi.Create().Dispose();
            _logDb.Create().Dispose();
            _logConsole.Create().Dispose();
            _logServer.Create().Dispose();

            _logWayfarerPrint.Create().Dispose();
            _logWayfarerEditor.Create().Dispose();

            _sw = new Stopwatch();
            _timer = new Timer();
            _sw.Start();

            // TODO: Consider creating a more sophisticated iterator for the Database methods
            _timer.Interval = 100;
            _timer.Elapsed += delegate { ProcessQueue(); };
            _timer.Enabled = true;

            _instantiated = true;
        }
        
        public static void Instantiate()
        {
            if (!_instantiated)
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
    
                _wayfarerDir = Directory.CreateDirectory(Paths.WayfarerLogPath);
                
                FileInfo[] wayfarerFiles = _wayfarerDir.GetFiles();
                foreach (FileInfo fi in wayfarerFiles)
                {
                    if (fi.Name == "___event.log"
                        || fi.Name == "editor.log")
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
                
                _logWayfarerPrint = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "___event.log"));
                _logWayfarerEditor = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "editor.log"));
    
                _logPrint.Create().Dispose();
                _logError.Create().Dispose();
                _logCrash.Create().Dispose();
                _logNetwork.Create().Dispose();
                _logUi.Create().Dispose();
                _logDb.Create().Dispose();
                _logConsole.Create().Dispose();
                _logServer.Create().Dispose();
    
                _logWayfarerPrint.Create().Dispose();
                _logWayfarerEditor.Create().Dispose();
    
                _sw = new Stopwatch();
                _timer = new Timer();
                _sw.Start();
    
                // TODO: Consider creating a more sophisticated iterator for the Database methods
                _timer.Interval = 100;
                _timer.Elapsed += delegate { ProcessQueue(); };
                _timer.Enabled = true;

                _instantiated = true;
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
        
        public static void Crash(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logPrint.FullName, alert));
            
            _printQueue.Enqueue(new PrintJob(_logCrash.FullName, crash));
            
            
            if (gdPrint)
            {
                GD.Print(alert);
                GD.Print(print);
                GD.Print(alert);
            }
            //Game.Self.Crash();
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
        
        public static void Editor(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = _sw.ElapsedMilliseconds + " | " + ParseFilePathToTypeName(path) + "." + method + " | " + "EDITOR: " + value;
            
            _printQueue.Enqueue(new PrintJob(_logWayfarerPrint.FullName, print));
            _printQueue.Enqueue(new PrintJob(_logWayfarerEditor.FullName, editor));
            
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

        private static string ParseFilePathToTypeName(string fullPath) // NOTE: This may not be very performant, it's a quick hack, we'll figure a better way later
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
        
        private static void ProcessQueue()
        {
            while (_printQueue.Count > 0)
            {
                PrintJob job = _printQueue.Dequeue();
                using (StreamWriter writer = new StreamWriter(job.LogPath, true))
                {
                    writer.WriteLine(job.Print);
                    writer.NewLine = "";
                    writer.Dispose();
                }
            }
        }

        private struct PrintJob
        {
            public readonly string LogPath;
            public readonly string Print;
            public PrintJob(string logPath, string print)
            {
                LogPath = logPath;
                Print = print;
            }
        }
    }
}