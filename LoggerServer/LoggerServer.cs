using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using LoggerSystem;
using NetworkingLogger;
namespace LoggerServer
{
    internal class LoggerServer
    {
        private int Port;
        private TcpListener Listener;
        private Dictionary<string, string> Systems = new Dictionary<string, string>();
        public LoggerServer(int port)
        {
            Port = port;
            Listener = new TcpListener(System.Net.IPAddress.Any, Port);
            foreach (var line in File.ReadAllLines("./conf.txt"))
            {
                Systems.Add(line.Split('=')[1], line.Split('=')[0]);
            }
        }

        /// <summary>
        /// As Blocking
        /// </summary>
        public void Listen()
        {
            Listener.Start();
            while (true)
            {
                try
                {
                    Task.Run(() => { HandelClient(Listener.AcceptTcpClient()); }).Start();
                }
                catch (Exception e) { }


            }
        }

        public virtual Task HandelClient(TcpClient tcpClient)
        {

            NetworkStream ms = tcpClient.GetStream();
            while (tcpClient.Connected)
            {
                Thread.Sleep(100);
                //Wait until data is there, then read and do this again
                while (ms.DataAvailable && ms.Socket.Available > 0)
                {
                    Thread.Sleep(100);
                    if (tcpClient.Connected == false)
                    {
                        return Task.CompletedTask;
                    }
                }

               
                MemoryStream memoryStream = new MemoryStream();




                byte[] buffer = new byte[512];


                do
                {
                    int numberOfBytesRead;
                    try
                    {
                        numberOfBytesRead = ms.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                    
                    memoryStream.Write(buffer, 0, numberOfBytesRead);
                }

                while ( ms.Socket.Available > 0);
                

                if (memoryStream.Length > 0)
                {
                    PacketV1 packetV1 = Serializer.ToPacket(memoryStream);

                    string name = Systems.FirstOrDefault(x => x.Key == packetV1.Token).Value ?? "Unknown System";

                    packetV1.Message = name + ": " + packetV1.Message;

                    switch (packetV1.Level)
                    {
                        case Levels.Debug:
                            LoggerSystem.Logger.Debug(packetV1.Message, DebugLogLevel.Debug);
                            continue;
                        case Levels.Info:
                            LoggerSystem.Logger.Information(packetV1.Message);
                            continue;
                        case Levels.Log:
                            LoggerSystem.Logger.Log(packetV1.Message);
                            continue;
                        case Levels.None:
                            LoggerSystem.Logger.Information(packetV1.Message);
                            continue;
                        case Levels.Warning:
                            LoggerSystem.Logger.Warning(packetV1.Message);
                            continue;
                    }

                }
                else
                {
                    LoggerSystem.Logger.Error("System: Empty data");
                }

            }

            return Task.CompletedTask;
        }

    }
}
