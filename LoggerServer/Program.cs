using static LoggerSystem.Logger;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using LoggerSystem;
using LoggerSystem.FileManagement;

namespace LoggerServer
{
    internal class Program : IDisposable
    {
        public static int Port = 27;

        static void Main(string[] args)
        {
            LoggerSystem.Logger.maxSaveDays = 14;
            LoggerSystem.Logger.Init();


            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            if (File.Exists("./conf.txt") == false)
            {
                Console.WriteLine("Create config file for client ids");
                File.Create("./conf.txt");
            }



            if (args.Length > 0)
            {
                if (args[0] == "add-client" && args.Length > 1)
                {
                    
                    string guid;
                    //Ensure only one guid for one client
                    while (true)
                    {
                        //Get Random guid
                        guid = Guid.NewGuid().ToString();

                        if (File.OpenText("./conf.txt").ToString().Contains(guid) == false)
                        {
                            File.AppendAllLines("./conf.txt", new string[] { $"{args[1]}=" + guid });
                            break;
                        }
                    }
                    Console.WriteLine("GUID: " + guid);
                    return;
                }

                try
                {
                    Port = int.Parse(args[0]);
                }
                catch
                {
                    LoggerSystem.Logger.Error($"System: Error with port to phrase use standard: 27/tcp");
                    Port = 27;
                }
            }

            LoggerServer loggerServer = new LoggerServer(Port);
            loggerServer.Listen();

            
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("exit");
            FileManager.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            FileManager.Dispose();

        }
    }
}
