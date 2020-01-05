using System;
using System.Collections.Generic;

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

        public static byte[] ToBytes(this uint i)
            => BitConverter.GetBytes(i);

        public static int FromBytes(this ref uint i, byte[] bytes, int startIndex)
        {
            i = BitConverter.ToUInt32(bytes, startIndex);
            return 4;
        }

        public static byte[] ToBytes(this ulong i)
            => BitConverter.GetBytes(i);

        public static int FromBytes(this ref ulong i, byte[] bytes, int startIndex)
        {
            i = BitConverter.ToUInt64(bytes, startIndex);
            return 8;
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
        
        public static int Clamp(this int i, int min, int max)
        {
            if (i < min)
                return min;
            if (i > max)
                return max;
            return i;
        }

        public static float Clamp(this float i, float min, float max)
        {
            if (i < min)
                return min;
            if (i > max)
                return max;
            return i;
        }

        public static float Pow(this int b, int exp)
        {
            float result = 1;

            if (exp > 0)
            {
                for (int i = 0; i < exp; i++)
                    result *= b;
            }
            else
            {
                for (int i = 0; i < -exp; i++)
                    result /= b;
            }

            return result;
        }

        public static bool IsPrime(this int i)
        {
            if (i < 2)
                return false;

            for (int j = 2; j * j <= i; j++)
            {
                if (i % j == 0)
                    return false;
            }

            return true;
        }

        public static List<int> Factor(this int x)
        {
            List<int> factors = new List<int>();

            for (int factor = 1; factor * factor <= x; ++factor)
            { 
                //test from 1 to the square root, or the int below it, inclusive.
                if (x % factor == 0)
                {
                    factors.Add(factor);
                    if (factor * factor != x)
                    { 
                        // Don't add the square root twice!
                        factors.Add(x / factor);
                    }
                }
            }

            return factors;
        }

        static Random random = new Random();

        public static T Random<T>(this IList<T> e) 
            => e[random.Next(0, e.Count)];

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

        public static string ToID(this uint i)
        {
            return i.ToString("x").PadLeft(8, '0').Insert(4, "-");
        }
    }
}
