using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Formations.Connection
{
    [Serializable]
    class ConnectionMessage
    {
        public byte[] Data { get; set; }
    }
}
