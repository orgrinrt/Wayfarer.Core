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
    public class Wayfarer
    {
        private DirectoryInfo _logDir;
        
        private FileInfo _logPrint;
        private FileInfo _logError;
        private FileInfo _logCrash;
        private FileInfo _logCrashDump;
        private FileInfo _logEditor;
        private FileInfo _logPebbles;
        
        private static Queue<Log.PrintJob> _printQueue = new Queue<Log.PrintJob>();
        private static Timer _timer;

        internal static Queue<Log.PrintJob> PrintQueue => _printQueue;
        
        private bool _initialized = false;
        internal bool Initialized => _initialized;
        
        public Wayfarer()
        {
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Couldn't initialize Wayfarer (child of Log)");
                
                Initialize();
            }
        }
        
        public void Initialize()
        {
            if (!_initialized)
            {
                _logDir = Directory.CreateDirectory(Paths.WayfarerLogPath);

                FileInfo[] files = _logDir.GetFiles();
                foreach (FileInfo fi in files)
                {
                    if (      fi.Name == "___event.log"
                           || fi.Name == "__error.log"
                           || fi.Name == "_crash.log"
                           || fi.Name == "_crash_dump.log"
                           || fi.Name == "editor.log"
                           || fi.Name == "pebbles.log")
                    {
                        fi.Delete();
                    }
                }
                
                _logPrint = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "___event.log"));
                _logError = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "__error.log"));
                _logCrash = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "_crash.log"));
                _logCrashDump = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "_crash_dump.log"));
                _logEditor = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "editor.log"));
                _logPebbles = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "pebbles.log"));
    
                _logPrint.Create().Dispose();
                _logError.Create().Dispose();
                _logCrash.Create().Dispose();
                _logEditor.Create().Dispose();
                _logPebbles.Create().Dispose();
                
                _timer = new Timer();
    
                // TODO: Consider creating a more sophisticated iterator for the Database methods
                _timer.Interval = Log.TickRate;
                _timer.Elapsed += delegate { ProcessQueue(); };
                _timer.Enabled = true;
                
                _initialized = true;
            }
        }
        
        public void Immediate(string value)
        {
            _logCrashDump.Create().Dispose();
        
            using (StreamWriter writer = new StreamWriter(_logCrashDump.FullName, true))
            {
                writer.NewLine = Log.Stopwatch.Elapsed +  " | dump below:";
                writer.NewLine = "";
                writer.WriteLine(value);
                writer.NewLine = "";
                writer.NewLine = "backlog (not processed yet):";

                foreach (Log.PrintJob job in _printQueue)
                {
                    writer.NewLine = job.Print;
                }
            
                writer.Dispose();
            }
        }
        
        public void Simple(string value, bool gdPrint = false)
        {
            string print = value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Print(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));

            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Print(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
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
        
        public void Error(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string error = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, error));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Error(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string error = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, error));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            
            
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
        
        public void Crash(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(alert);
                GD.Print(print);
                GD.Print(alert);
            }
            
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, crash, true)); // we cause an exception with the last bool parameter here
        }
        
        public void Crash(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, crash));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(print);
                GD.Print("");
                GD.Print(e.Message);
                GD.Print("in: " + e.Source);
                GD.Print(e.StackTrace);
                GD.Print("");
            }
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert, true)); // we cause an exception with the last bool parameter here
        }
        
        public void Editor(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "EDITOR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, editor));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Editor(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "EDITOR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, editor));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            
            
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
        
        public void EditorError(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, editor));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, print));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void EditorError(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, editor));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            
            
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
        
        public void Pebbles(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string pebbles = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, pebbles));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Pebbles(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string pebbles = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, pebbles));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, ""));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, e.Message));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, "in: " + e.Source));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, e.StackTrace));
            PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, ""));
            
            
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
        
        internal static void ProcessQueue()
        {
            while (_printQueue.Count > 0)
            {
                Log.PrintJob job = _printQueue.Dequeue();
                using (StreamWriter writer = new StreamWriter(job.LogPath, true))
                {
                    writer.WriteLine(job.Print);
                    writer.NewLine = "";
                    if (job.Crash || job.Exception != null)
                    {
                        if (job.Crash && job.Exception == null)
                        {
                            throw new Exception("Log.ProcessQueue() Demanded that we crash (above message)");
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
    }
}