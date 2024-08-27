using LoggerSystem;
using LoggerSystem.ConsoleSystem;
using LoggerSystem.FileManagement;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerSystem
{
    public static class Logger
    {
        

        public static Levels minLogLevel = Levels.None;
        /// <summary>
        /// Max save days when file reached the date, it will be deleted
        /// </summary>
        public static int maxSaveDays = 2;

        private static bool network = false;
        public static string ip { get; internal set; }
        public static int port { get; internal set; }
        public static string token { get; internal set; }
        public static bool init { get; internal set; } = false;
        private static Thread worker = null;
        public static void Init()
        {
            FileManagement.FileManager.OpenLogFile();
            init = true;
            worker = new Thread(SaveToFile);
            worker.Name = "WokerSaver";
            worker.Start();
            FileManager.DeleteOldFiles();
        }
        public static void Init(string IP, int Port)
        {
            FileManagement.FileManager.OpenLogFile();
            init = true;
            worker = new Thread(SaveToFile);
            worker.Name = "WokerSaver";
            worker.Start();
            FileManager.DeleteOldFiles();

        }

        public static async void Init(string Token, string IP, int Port)
        {
            init = true;
            worker = new Thread(SaveToFile);
            worker.Name = "WokerSaver";
            worker.Start();
            token = Token;
            ip = IP;
            port = Port;
            init = true;
            //Non Blocking, if server is down
            Task.Run( () =>  FileManagement.FileManager.Init());
        }

        public static void SaveToFile()
        {
            while (init)
            {
                Thread.Sleep(2000);
                FileManager.Save();
            }
        }

        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            if (init == false)
            {
                return;
            }
            if (Levels.Log >= minLogLevel)
            {
                FileManager.WriteToFile(message, Levels.Log);
            }
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static void Warning(string message)
        {
            if (init == false)
            {
                return;
            }
            if (Levels.Warning >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Warning);
            }
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            if (init == false)
            {
                return;
            }
            if (Levels.Error >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Error);
            }
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static void Information(string message)
        {
            if (init == false)
            {
                return;
            }
            if (Levels.None >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.None);
            }
        }

        /// <summary>
        /// Write a message to the console and also saves to a file, if the program is in debug, then it shows on the debug console 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        /// <param name="level"></param>
        public static void Debug(string message, DebugLogLevel level = DebugLogLevel.Debug, string category = "common")
        {
            if (init == false)
            {
                return;
            }
            if (Levels.None >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Debug);
                Debugger.Log((int)level, category, message);
            }
        }

        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static Task LogAsync(string message)
        {
            if (init == false)
            {
                return Task.CompletedTask;
            }
            if (Levels.Log >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Log);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static Task WarningAsync(string message)
        {
            if (init == false)
            {
                return Task.CompletedTask;
            }
            if (Levels.Warning >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Warning);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static Task ErrorAsync(string message)
        {
            if (init == false)
            {
                return Task.CompletedTask;
            }
            if (Levels.Error >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Error);
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// Write a message to the console and also saves to a file
        /// </summary>
        /// <param name="message"></param>
        public static Task InformationAsync(string message)
        {
            if (init == false)
            {
                return Task.CompletedTask;
            }
            if (Levels.None >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.None);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Write a message to the console and also saves to a file, if the program is in debug, then it shows on the debug console 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="category"></param>
        /// <param name="level"></param>
        public static Task DebugAsync(string message, DebugLogLevel level = DebugLogLevel.Debug, string category = "common")
        {
            if (init == false)
            {
                return Task.CompletedTask;
            }
            if (Levels.None >= minLogLevel)
            {
                FileManagement.FileManager.WriteToFile(message, Levels.Debug);
                Debugger.Log((int)level, category, message);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Call this function when the app closes 
        /// </summary>
        public static void Close()
        {
            init = false;
            FileManagement.FileManager.Close();

            
            if (worker != null)
            {
                //Handel error
                try
                {
                    worker.Abort();
                }
                catch (Exception ex)
                {
                    
                }
                }

            FileManager.Dispose();
        }
    }
}