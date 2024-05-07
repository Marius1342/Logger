using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        public LoggerServer(int port)
        {
            Port = port;
            Listener = new TcpListener(System.Net.IPAddress.Any,Port);
        }

        /// <summary>
        /// As Blocking
        /// </summary>
        public void Listen()
        {
            Listener.Start();
            while(true)
            {
                
                HandelClient(Listener.AcceptTcpClient());
                
                

            }
        }

        public virtual Task HandelClient(TcpClient tcpClient)
        {

            NetworkStream ms = tcpClient.GetStream();
            byte[] buffer = new byte[512];
            MemoryStream memoryStream = new MemoryStream();

            int numberOfBytesRead = ms.Read(buffer, 0, buffer.Length);
            memoryStream.Write(buffer, 0, numberOfBytesRead);

            while (numberOfBytesRead > 0)
            {
                numberOfBytesRead = ms.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, numberOfBytesRead);
            }

            if(memoryStream.Length > 0) {
                PacketV1 packetV1 = Serializer.ToPacket(memoryStream);


            }
            else
            {
                Logger.Error("System: Empty data");
            }

            

            return Task.CompletedTask;
        }

    }
}
