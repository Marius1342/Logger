using LoggerSystem;
using NetworkingLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Logger.Network
{
    internal class Client
    {
        private string Token;
        private string IP;
        public int Port;
        public Client(string Ip, int Port, string Token)
        {
            this.IP = Ip;
            this.Port = Port;
            this.Token = Token;
        }

        public void Send(Levels levels, string msg)
        {
            PacketV1 packetV1 = new PacketV1()
            {
                Level = levels,
                Message = msg,
                UUId = this.Token,
            };

            byte[] arr = Serializer.ToByteArray(packetV1);


            //Connect and send
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(IP, Port);
            }catch(Exception ex)
            {
                LoggerSystem.Logger.Error($"Ex: {ex.Message}");
                return;
            }
            try { 
            tcpClient.GetStream().Write(arr, 0, arr.Length);
            }
            catch (Exception ex)
            {
                LoggerSystem.Logger.Error($"Ex: {ex.Message}");
                return;
            }
            tcpClient.Close();
        }

    }
}
