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

        public static PacketV1 ToPacket(MemoryStream packet)
        {
            
            packet.Position = 0;
            PacketV1 packetV1;
            //Unsafe
            /*IFormatter formatter = new BinaryFormatter();
            
            try
            {
                packetV1 = (PacketV1)formatter.Deserialize(packet);
            }
            catch (Exception ex)
            {
                LoggerSystem.Logger.Error($"System: Error with obj to serialize: {ex.Message}: {ex.InnerException}");
                return new PacketV1()
                {
                    Token = "0",
                    Message = "Error with obj to serialize",
                    Level = Levels.Error
                };
            }*/

            string dataJson = Encoding.UTF8.GetString(packet.ToArray());

            packetV1 = (PacketV1)JsonSerializer.Deserialize<PacketV1>(dataJson);

            return packetV1;
        }
    }
}
