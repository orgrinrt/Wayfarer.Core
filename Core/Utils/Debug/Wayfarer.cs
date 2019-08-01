using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;
using Wayfarer.Core.Utils.Files;
using Directory = System.IO.Directory;
using Path = System.IO.Path;

namespace Wayfarer.Core.Utils.Debug
{
    public class Wayfarer
    {
        private DirectoryInfo _logDir;
        
        private FileInfo _logPrint;
        private FileInfo _logError;
        private FileInfo _logCrash;
        private FileInfo _logEditor;
        private FileInfo _logPebbles;
        
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
                           || fi.Name == "editor.log"
                           || fi.Name == "pebbles.log")
                    {
                        fi.Delete();
                    }
                }
                
                _logPrint = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "___event.log"));
                _logError = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "__error.log"));
                _logCrash = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "_crash.log"));
                _logEditor = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "editor.log"));
                _logPebbles = new FileInfo(Path.Combine(Paths.WayfarerLogPath, "pebbles.log"));
    
                _logPrint.Create().Dispose();
                _logError.Create().Dispose();
                _logCrash.Create().Dispose();
                _logEditor.Create().Dispose();
                _logPebbles.Create().Dispose();
                
                _initialized = true;
            }
        }
        
        public void Immediate(string value)
        {
            // TODO: This will be used in Exceptions and catch statements
            // We want this to immediately write the last line before crash
        }
        
        public void Simple(string value, bool gdPrint = false)
        {
            string print = value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Print(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));

            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Print(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
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
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, error));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Error(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string error = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, error));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logError.FullName, ""));
            
            
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
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(alert);
                GD.Print(print);
                GD.Print(alert);
            }
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, crash, true)); // we cause an exception with the last bool parameter here
        }
        
        public void Crash(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string alert = "!!!!!!!!!!!!!";
            string crash = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "CRASH: " + value;
            
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, crash));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logCrash.FullName, ""));
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            if (gdPrint)
            {
                GD.Print(print);
                GD.Print("");
                GD.Print(e.Message);
                GD.Print("in: " + e.Source);
                GD.Print(e.StackTrace);
                GD.Print("");
            }
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, alert, true)); // we cause an exception with the last bool parameter here
        }
        
        public void Editor(string value, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, editor));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Editor(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string editor = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, editor));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logEditor.FullName, ""));
            
            
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
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, pebbles));
            
            if (gdPrint)
            {
                GD.Print(print);
            }
        }
        
        public void Pebbles(string value, Exception e, bool gdPrint = false, [CallerMemberName]string method = "", [CallerFilePath] string path = "")
        {
            string pebbles = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + value;
            string print = Log.Stopwatch.ElapsedMilliseconds + " | " + Log.ParseFilePathToTypeName(path) + "." + method + " | " + "ERROR: " + value;
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, print));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPrint.FullName, ""));
            
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, pebbles));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, ""));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, e.Message));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, "in: " + e.Source));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, e.StackTrace));
            Log.PrintQueue.Enqueue(new Log.PrintJob(_logPebbles.FullName, ""));
            
            
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
    }
}