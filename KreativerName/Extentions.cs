using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Grid;

namespace KreativerName
{
    public static class Extentions
    {
        public static byte[] ToBytes(this int i)
            => BitConverter.GetBytes(i);
    }
}
