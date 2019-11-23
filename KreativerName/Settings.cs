using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    struct Settings : IBytes
    {
        public bool ShowMoves;

        public static Settings Current { get; set; }

        public int FromBytes(byte[] bytes, int startIndex)
        {

            return 0;
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.Add(ShowMoves ? (byte)1 : (byte)0);

            return bytes.ToArray();
        }
    }
}
