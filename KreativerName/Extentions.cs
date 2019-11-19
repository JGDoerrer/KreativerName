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

        public static string ToRomanNumerals(this int i)
        {
            List<Tuple<char, int>> alphabet = new List<Tuple<char, int>>()
            {
                new Tuple<char, int>( 'M', 1000 ),
                new Tuple<char, int>( 'D', 500 ),
                new Tuple<char, int>( 'C', 100 ),
                new Tuple<char, int>( 'L', 50 ),
                new Tuple<char, int>( 'X', 10 ),
                new Tuple<char, int>( 'V', 5 ),
                new Tuple<char, int>( 'I', 1 )
            };

            string s = "";
            foreach (var item in alphabet)
            {
                while (i >= item.Item2)
                {
                    s += item.Item1;
                    i -= item.Item2;
                }
            }
            return s;
        }
    }
}
