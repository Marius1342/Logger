using LoggerSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting.Contexts;
using Newtonsoft.Json;

namespace LoggerSystem.NetworkingLogger
{
    public static class Serializer
    {
        public static byte[] ToByteArray(PacketV1 packet)
        {
            
            string data = JsonConvert.SerializeObject(packet, Formatting.Indented);

            return Encoding.UTF8.GetBytes(data);
        }
        public static byte[] ToByteArray(PacketV2 packet)
        {
            byte[] bytes;
            string data = JsonConvert.SerializeObject(packet, Formatting.Indented);

            bytes = Encoding.UTF8.GetBytes(data);

            return bytes;
        }


        public static PacketV1 ToPacket(MemoryStream packet)
        {
            packet.Position = 0;
            PacketV1 packetV1;

            packet.Position = 0;
            using (var sr = new StreamReader(packet, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(sr))
            {
                packetV1 = new JsonSerializer().Deserialize<PacketV1>(jsonReader);
            }
            return packetV1;

            //packet.Position = 0;
            //PacketV1 packetV1;


            //string dataJson = Encoding.UTF8.GetString(packet.ToArray());

            //packetV1 = (PacketV1)JsonConvert.DeserializeObject<PacketV1>(dataJson);

            //return packetV1;
        }

        public static PacketV2 ToPacketV2(MemoryStream packet)
        {

            packet.Position = 0;
            PacketV2 packetV2;

            packet.Position = 0;
            using (var sr = new StreamReader(packet, Encoding.UTF8))
            using (var jsonReader = new JsonTextReader(sr))
            {
                packetV2 = new JsonSerializer().Deserialize<PacketV2>(jsonReader);
            }
            return packetV2;

            //string dataJson = Encoding.UTF8.GetString(packet.ToArray());

            //packetV2 = (PacketV2)JsonConvert.DeserializeObject<PacketV2>(dataJson);

            //return packetV2;
        }

    }
}
