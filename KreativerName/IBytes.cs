using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public interface IBytes
    {
        byte[] ToBytes();
        void FromBytes(byte[] bytes, int startIndex);
    }
}
