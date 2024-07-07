using LoggerSystem.ConsoleSystem;
using LoggerSystem.NetworkingLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace LoggerSystem.FileManagement
{
    public class FileManager
    {
        public static FileStream logFile;
        public static object logFileLock = new object();
        private static NetworkStream logStream;
        private static TcpClient tcpClient;
        private static Thread sender;
        private static List<PacketV1> packetV1s = new List<PacketV1>();
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
            get { return DateTime.Now.ToString("MM-dd-yyyy") + ".txt"; }


        }

        /// <summary>
        /// Await this task, if you want ensure, that client is connected 
        /// </summary>
        /// <returns></returns>
        public static void Init()
        {

            tcpClient = new TcpClient();
            while (tcpClient.Connected == false)
            {
                try
                {
                    tcpClient.Connect(Logger.ip, Logger.port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FATAL ERROR: {ex.Message}:{ex.InnerException}");
                }
                Thread.Sleep(5000);
            }
            logStream = tcpClient.GetStream();

            sender = new Thread(Sender);
            sender.Start();


        }

        private static void Sender()
        {
            while (true)
            {
                PacketV1 pk = null;
                while (pk == null)
                {
                    Thread.Sleep(750);
                    lock (packetV1s)
                    {
                        pk = packetV1s.FirstOrDefault();
                        if (pk != null)
                        {
                            packetV1s.RemoveAt(0);
                        }
                    }
                }


                byte[] buffer = Serializer.ToByteArray(pk);


                try
                {
                    logStream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FATAL ERROR: {ex.Message}:{ex.InnerException}");
                    lock (packetV1s)
                    {
                        packetV1s.Add(pk);
                    }
                }

                tcpClient = new TcpClient();
                
                //Auto reconnect, only one message per client
                while (tcpClient.Connected == false)
                {
                    try
                    {
                        tcpClient.Connect(Logger.ip, Logger.port);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"FATAL ERROR: {ex.Message}:{ex.InnerException}");
                        Thread.Sleep(5000);
                    }
                }

                //Set the log stream 
                logStream = tcpClient.GetStream();
            }
        }

        public static void Close()
        {
            while (packetV1s.Count > 0)
            {
                Thread.Sleep(50);
            }
            tcpClient.Close();
            sender.Abort();
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

            //Only use the Server
            if (Logger.ip != null)
            {
                PacketV1 packetV1 = new PacketV1()
                {
                    Message = message,
                    Level = level,
                    Token = Logger.token,
                };

                //Thread save
                lock (logFileLock)
                {
                    packetV1s.Add(packetV1);
                }
                if (tcpClient != null && tcpClient.Connected)
                {
                    return;
                }

            }


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
                if (logFile == null)
                {
                    logFile = new FileStream(GetFullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    logFile.Flush();
                }





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
