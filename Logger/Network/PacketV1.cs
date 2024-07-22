using LoggerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LoggerSystem.NetworkingLogger
{
    [Serializable]

    public class PacketV1 
    {
        public string Message { get; set; }
        public Levels Level { get; set; }
        public string Token {  get; set; }

    }
}
