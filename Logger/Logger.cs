using LoggerSystem;
using LoggerSystem.FileManagement;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerSystem
{
    public static class Logger
    {
        public static Levels minLogLevel = Levels.None;
        public static int maxSaveDays = 2;
        private static bool init = false;
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

        public static void SaveToFile()
        {
            while (init)
            {
                Thread.Sleep(150);
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
        /// Call this function when the app closes 
        /// </summary>
        public static void Close()
        {
            init = false;
            worker.Abort();
            FileManager.Dispose();
        }
    }
}