using System;
using System.Collections.Generic;
using System.Linq;

namespace KreativerName
{
    public static class Extentions
    {
        #region Bytes

        public static byte[] ToBytes(this int i)
            => BitConverter.GetBytes(i);

        public static int FromBytes(this ref int i, byte[] bytes, int startIndex)
        {
            i = BitConverter.ToInt32(bytes, startIndex);
            return 4;
        }

        public static byte[] ToBytes(this TimeSpan ts)
            => BitConverter.GetBytes(ts.Ticks);

        public static int FromBytes(this ref TimeSpan ts, byte[] bytes, int startIndex)
        {
            long ticks = BitConverter.ToInt64(bytes, startIndex);
            ts = new TimeSpan(ticks);
            return 8;
        }

        public static byte[] ToBytes(this DateTime dt)
            => BitConverter.GetBytes(dt.ToBinary());

        public static int FromBytes(this ref DateTime dt, byte[] bytes, int startIndex)
        {
            long data = BitConverter.ToInt64(bytes, startIndex);
            dt = DateTime.FromBinary(data);
            return 8;
        }

        #endregion

        public static string ToRoman(this int i)
        {
            if (i >= 1000) return "M" + (i - 1000).ToRoman();
            if (i >= 900) return "CM" + (i - 900).ToRoman();
            if (i >= 500) return "D" + (i - 500).ToRoman();
            if (i >= 400) return "CD" + (i - 400).ToRoman();
            if (i >= 100) return "C" + (i - 100).ToRoman();
            if (i >= 90) return "XC" + (i - 90).ToRoman();
            if (i >= 50) return "L" + (i - 50).ToRoman();
            if (i >= 40) return "XL" + (i - 40).ToRoman();
            if (i >= 10) return "X" + (i - 10).ToRoman();
            if (i >= 9) return "IX" + (i - 9).ToRoman();
            if (i >= 5) return "V" + (i - 5).ToRoman();
            if (i >= 4) return "IV" + (i - 4).ToRoman();
            if (i >= 1) return "I" + (i - 1).ToRoman();
            return "";
        }
    }
}
