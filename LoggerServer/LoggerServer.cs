using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using LoggerSystem;
using LoggerSystem.FileManagement;
using LoggerSystem.NetworkingLogger;
namespace LoggerServer
{
    internal class LoggerServer : IDisposable
    {
        private int Port;
        private TcpListener Listener;
        private Dictionary<string, string> Systems = new Dictionary<string, string>();
        public LoggerServer(int port)
        {
            Port = port;
            Listener = new TcpListener(System.Net.IPAddress.Any, Port);

            //When new conf.txt is gen. then throws error
            try
            {
                foreach (var line in File.ReadAllLines("./conf.txt"))
                {
                    Systems.Add(line.Split('=')[1], line.Split('=')[0]);
                }
            }
            catch (Exception ex)
            {
            }
            Task.Delay(2000);
        }

        /// <summary>
        /// As Blocking
        /// </summary>
        public void Listen()
        {
            Listener.Start();
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    Task.Run(() => { HandelClient(Listener.AcceptTcpClient()); }).Start();
                }
                catch (Exception e) { }

                if (i % 15 == 0)
                {

                    foreach (var line in File.ReadAllLines("./conf.txt"))
                    {
                        if (Systems.ContainsKey(line.Split('=')[1]) == false)
                        {
                            Systems.Add(line.Split('=')[1], line.Split('=')[0]);
                        }

                    }
                    i = 0;
                }
                Thread.Sleep(125);
            }
            FileManager.Close();
        }

        public virtual Task HandelClient(TcpClient tcpClient)
        {

            NetworkStream ms = tcpClient.GetStream();
            while (tcpClient.Connected)
            {
                //Wait until data is there, then read and do this again
                while (ms.DataAvailable && ms.Socket.Available < 0)
                {
                    Thread.Sleep(125);
                    if (tcpClient.Connected == false)
                    {
                        return Task.CompletedTask;
                    }
                }


                MemoryStream memoryStream = new MemoryStream();




                byte[] buffer = new byte[512];

                //ms.ReadTimeout = 2500;
                do
                {
                    int numberOfBytesRead;
                    try
                    {
                        numberOfBytesRead = ms.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception e)
                    {
                        LoggerSystem.Logger.Error($"System Timeout read data");
                        break;
                    }

                    memoryStream.Write(buffer, 0, numberOfBytesRead);
                }

                while (ms.Socket.Available > 0);


                if (memoryStream.Length > 0)
                {
                    PacketV2 packet = new PacketV2();
                    try
                    {
                        packet = Serializer.ToPacketV2(memoryStream);
                    }
                    catch (Exception e)
                    {
                        PacketV1 packet1 = Serializer.ToPacket(memoryStream);
                        packet.Message = packet1.Message;
                        packet.Level = packet1.Level;
                        packet.Token = packet1.Token;
                        LoggerSystem.Logger.Information($"System with token: {packet.Token} is using old version of packets");
                    }
                    if (packet.Command == PacketV2.Commands.GetXmlFiles)
                    {
                        FileManager.NewXmlFile();
                        LoggerSystem.Logger.Information("Scan for XML Files Network download");
                        string[] files = FileManager.GetXmlFiles;
                        PacketV2 packetV2 = new PacketV2();
                        packetV2.Command = PacketV2.Commands.AllXmlFiles;
                        packetV2.Data = files;


                        LoggerSystem.Logger.Information($"Found {files.Length} files");
                        //Send 
                        byte[] res = Serializer.ToByteArray(packetV2);

                        ms.Write(res, 0, res.Length);

                        ms.Flush();

                        //Latenz usw network
                        Thread.Sleep(1500);

                        tcpClient.Close();
                        break;
                    }

                    if (packet.Command == PacketV2.Commands.GetFileContent)
                    {
                        List<string>files = FileManager.GetXmlFiles.ToList();
                        byte[] res;

                        PacketV2 packetV2 = new PacketV2();
                        packetV2.Command = PacketV2.Commands.FileContent;
                        
                        //Check if user wants really the log file or system file
                        if (packet.Data.Length < 1 && files.Exists( x => x == packet.Data[0]) == false)
                        {
                            LoggerSystem.Logger.Information($"File not found: {packet.Data[0]}");

                            res = Serializer.ToByteArray(packetV2);

                            ms.Write(res, 0, res.Length);
                            ms.Flush();

                            //Latenz usw network
                            Thread.Sleep(500);
                            tcpClient.Close();
                            break;
                        }
                        
                        packetV2.Data = File.ReadLines(packet.Data[0]).ToArray();
                       
                        //Send 
                        res = Serializer.ToByteArray(packetV2);
                        LoggerSystem.Logger.Information($"Try Send File lines: {packetV2.Data.Length}");
                        ms.Write(res, 0, res.Length);
                        ms.Flush();
                        LoggerSystem.Logger.Information($"Send File lines: {packetV2.Data.Length}");
                        Thread.Sleep(500);
                        tcpClient.Close();
                        break;
                    }


                    string name = Systems.FirstOrDefault(x => x.Key == packet.Token).Value ?? "Unknown System";

                    packet.Message = name + ": " + packet.Message;

                    switch (packet.Level)
                    {
                        case Levels.Debug:
                            LoggerSystem.Logger.Debug(packet.Message, DebugLogLevel.Debug);
                            continue;
                        case Levels.Info:
                            LoggerSystem.Logger.Information(packet.Message);
                            continue;
                        case Levels.Log:
                            LoggerSystem.Logger.Log(packet.Message);
                            continue;
                        case Levels.None:
                            LoggerSystem.Logger.Information(packet.Message);
                            continue;
                        case Levels.Warning:
                            LoggerSystem.Logger.Warning(packet.Message);
                            continue;
                        case Levels.Error:
                            LoggerSystem.Logger.Error(packet.Message);
                            continue;
                    }

                }
                else
                {
                    Console.WriteLine("System: Empty data");
                    tcpClient.Close();
                }

            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);



            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            FileManager.Close();
        }


    }
}
