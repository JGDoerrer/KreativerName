using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KreativerName.Grid
{
#nullable enable

    /// <summary>
    /// Eine Klasse zum Speichen einer Hexagonalen Fläche.
    /// Es werden axiale Koordinaten verwendet.
    /// x: + => rechts  - => links
    /// y: + => unten rechts  - => oben links
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HexGrid<T> : IEnumerable<T>, IBytes where T : struct, IBytes
    {
        public HexGrid()
        {
            dictonary = new Dictionary<HexPoint, T>();
        }

        Dictionary<HexPoint, T> dictonary;

        public T? this[HexPoint pos]
        {
            get
            {
                if (dictonary.ContainsKey(pos))
                    return dictonary[pos];

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (dictonary.ContainsKey(pos))
                        dictonary[pos] = (T)value;
                    else
                        dictonary.Add(pos, (T)value);
                }
                else if (dictonary.ContainsKey(pos))
                {
                    dictonary.Remove(pos);
                }

            }
        }
        public T? this[int x, int y]
        {
            get => this[new HexPoint(x, y)];
            set => this[new HexPoint(x, y)] = value;
        }

        public void Clear()
        {
            dictonary.Clear();
        }

        public bool Contains(HexPoint point)
            => dictonary.ContainsKey(point);

        public IEnumerator<T> GetEnumerator()
        {
            return dictonary.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(dictonary.Count));
            foreach (var item in dictonary)
            {
                bytes.AddRange(item.Key.ToBytes());
                bytes.AddRange(item.Value.ToBytes());
            }

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            Clear();

            int byteCount = 0;

            int count = BitConverter.ToInt32(bytes, startIndex);
            startIndex += 4;
            byteCount += 4;

            for (int i = 0; i < count; i++)
            {
                HexPoint key = new HexPoint();
                int lengthKey = key.FromBytes(bytes, startIndex);
                startIndex += lengthKey;
                byteCount += lengthKey;

                T value = new T();
                int lengthVal = value.FromBytes(bytes, startIndex);
                startIndex += lengthVal;
                byteCount += lengthVal;

                dictonary.Add(key, value);
            }

            return byteCount;
        }
    }
}