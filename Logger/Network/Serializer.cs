using LoggerSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingLogger
{
    public static class Serializer
    {
        public static byte[] ToByteArray(PacketV1 packet)
        {
            byte[] bytes;

            IFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, packet);
                bytes = ms.ToArray();
            }


            return bytes;
        }

        public static PacketV1 ToPacket(MemoryStream packet)
        {
            PacketV1 packetV1;
            IFormatter formatter = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    packetV1 = (PacketV1)formatter.Deserialize(packet);
                }
                catch 
                {
                    Logger.Error("System: Error with obj to serialize");
                    return new PacketV1()
                    {
                        UUId = "0",
                        Message = "Error with obj to serialize",
                        Level = Levels.Error
                    };
                }
                
            }

            return packetV1;
        }
    }
}
