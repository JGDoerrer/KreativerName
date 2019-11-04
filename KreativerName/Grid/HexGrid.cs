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
    public class HexGrid<T> : IDictionary<HexPoint, T>, IBytes where T : struct, IBytes
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

        #region Interfaces

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(dictonary.Count));
            foreach (KeyValuePair<HexPoint, T> item in dictonary)
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

        public IEnumerator<T> GetEnumerator()
        {
            return dictonary.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(HexPoint key) => dictonary.ContainsKey(key);
        public void Add(HexPoint key, T value) => dictonary.Add(key, value);
        public bool Remove(HexPoint key) => dictonary.Remove(key);
        public bool TryGetValue(HexPoint key, out T value) => dictonary.TryGetValue(key, out value);

        T IDictionary<HexPoint, T>.this[HexPoint key] { get => dictonary[key]; set => dictonary[key] = value; }

        public ICollection<HexPoint> Keys => dictonary.Keys;

        public ICollection<T> Values => dictonary.Values;

        public void Add(KeyValuePair<HexPoint, T> item) => dictonary.Add(item.Key, item.Value);
        public bool Contains(KeyValuePair<HexPoint, T> item) => dictonary.Contains(item);
        public void CopyTo(KeyValuePair<HexPoint, T>[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(KeyValuePair<HexPoint, T> item) => dictonary.Remove(item.Key);

        public int Count => dictonary.Count;

        public bool IsReadOnly => false;

        IEnumerator<KeyValuePair<HexPoint, T>> IEnumerable<KeyValuePair<HexPoint, T>>.GetEnumerator() => dictonary.GetEnumerator();

        #endregion
    }
}