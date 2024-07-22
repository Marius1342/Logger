using LoggerSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Runtime.Remoting.Contexts;
using System.Text.Json.Serialization.Metadata;

namespace LoggerSystem.NetworkingLogger
{
    public static class Serializer
    {
        public static byte[] ToByteArray(PacketV1 packet)
        {
            byte[] bytes;

            //Unsafe
            /* IFormatter formatter = new BinaryFormatter();

             using (MemoryStream ms = new MemoryStream())
             {
                 formatter.Serialize(ms, packet);
                 bytes = ms.ToArray();
             }*/
            var options = new JsonSerializerOptions { WriteIndented = true };
            string data = JsonSerializer.Serialize<PacketV1>(packet, options);

            bytes = Encoding.UTF8.GetBytes(data);

            return bytes;
        }
        public static byte[] ToByteArray(PacketV2 packet)
        {
            byte[] bytes;

            //Unsafe
            /* IFormatter formatter = new BinaryFormatter();

             using (MemoryStream ms = new MemoryStream())
             {
                 formatter.Serialize(ms, packet);
                 bytes = ms.ToArray();
             }*/
            var options = new JsonSerializerOptions { WriteIndented = true };
            string data = JsonSerializer.Serialize<PacketV2>(packet, options);

            bytes = Encoding.UTF8.GetBytes(data);

            return bytes;
        }


        public static PacketV1 ToPacket(MemoryStream packet)
        {
            
            packet.Position = 0;
            PacketV1 packetV1;
            

            string dataJson = Encoding.UTF8.GetString(packet.ToArray());

            packetV1 = (PacketV1)JsonSerializer.Deserialize<PacketV1>(dataJson);

            return packetV1;
        }

        public static PacketV2 ToPacketV2(MemoryStream packet)
        {

            packet.Position = 0;
            PacketV2 packetV2;


            string dataJson = Encoding.UTF8.GetString(packet.ToArray());

            packetV2 = (PacketV2)JsonSerializer.Deserialize<PacketV2>(dataJson);

            return packetV2;
        }

    }
}
