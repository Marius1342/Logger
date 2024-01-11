using LoggerSystem.ConsoleSystem;
using System;
using System.IO;
using System.Text;
namespace LoggerSystem.FileManagement
{
    public class FileManager
    {
        public static FileStream logFile;
        public static object logFileLock = new object();
        public static int writtenLines
        {
            get; private set;
        }
        private static string currentFileName = string.Empty;
        public static string GetFullPath
        {
            get { return Path.Combine(GetAppPath, "logs", GetFile); }
        }
        public static string GetAppPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string GetFile
        {
            get { return DateTime.Now.ToString("dd-MM-yyyy") + ".txt"; }

        }
        /// <summary>
        /// Write the data into a file and to the console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void WriteToFile(string message, Levels level)
        {

            string time = DateTime.Now.ToString("HH:mm:ss:f dd-MM-yyyy");
            string log = $"[{LevelToMessage(level)}] {time}: {message}";

            ConsoleHelper.WriteToConsole(log, level);

            if (logFile == null)
            {
                ConsoleHelper.WriteToConsole("Error logFile is null", Levels.Error);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(log + Environment.NewLine);

            lock (logFileLock)
            {
                logFile.Write(bytes, 0, bytes.Length);
                writtenLines++;
            }
            if (currentFileName != logFile.Name)
            {
                //Create new file, new day
                lock (logFileLock)
                {
                    OpenLogFile();
                    writtenLines = 0;
                }
            }
        }

        public static string LevelToMessage(Levels level)
        {
            switch (level)
            {
                case Levels.None:
                    return "Information";
                case Levels.Log:
                    return "Log";
                case Levels.Warning:
                    return "Warning";
                case Levels.Error:
                    return "Error";
                case Levels.Debug:
                    return "Debug";
                default:
                    return "Unknown";
            }
        }

        public static bool OpenLogFile()
        {
            if (Directory.Exists(Path.Combine(GetAppPath, "logs")) == false)
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(GetAppPath, "logs"));
                }
                catch (Exception ex)
                {
                    WriteToFile($"Fatal error: {ex.Message}", Levels.Error);
                    return false;
                }
            }

            if (File.Exists(GetFullPath) == false)
            {
                try
                {
                    logFile = new FileStream(GetFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                catch (Exception ex)
                {
                    WriteToFile($"Fatal error: {ex.Message}", Levels.Error);
                    return false;
                }

            }
            else
            {
                logFile = new FileStream(GetFullPath, FileMode.Open, FileAccess.ReadWrite);


                logFile.Flush();
            }
            writtenLines = 0;
            currentFileName = logFile.Name;
            return true;
        }

        public static void DeleteOldFiles()
        {
            string[] files = Directory.GetFiles(Path.Combine(GetAppPath, "logs"));

            int i = -1;
            foreach (string file in files)
            {
                i++;
                //Create DateTime from name
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 4);
                bool success = DateTime.TryParse(name, out DateTime time);
                if (success == false)
                {
                    WriteToFile($"Error with file:{file}", Levels.Error);
                    continue;
                }
                //Check if time limit is reached
                if ((DateTime.Now - time).Days > Logger.maxSaveDays)
                {
                    //Delete file 
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch (Exception ex)
                    {
                        WriteToFile($"Error while deleting file: {file} reason: {ex.Message}", Levels.Error);
                    }
                }
            }
        }
        public static void Save()
        {
            if (writtenLines > 3)
            {
                logFile.Flush();
            }
        }
        public static void Dispose()
        {
            logFile.Flush();
            logFile.Close();
            logFile.Dispose();
        }
    }
}
