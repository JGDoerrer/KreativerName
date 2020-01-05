using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KreativerName.Grid
{
#nullable enable

    /// <summary>
    /// Stores a hexagonal grid using axial coordinates.
    /// x: + => east  - => west
    /// y: + => south east  - => north west
    /// </summary>
    /// <typeparam name="T">The type of the object to store.</typeparam>
    public class HexGrid<T> : IDictionary<HexPoint, T>, IBytes where T : struct, IBytes
    {
        /// <summary>
        /// Creates a new HexGrid.
        /// </summary>
        public HexGrid()
        {
            dictonary = new Dictionary<HexPoint, T>();
        }

        Dictionary<HexPoint, T> dictonary;

        /// <summary>
        /// Gets or sets the object at the specified position.
        /// Returns null if there is no object at the position.
        /// </summary>
        /// <param name="pos">The position to access</param>
        /// <returns>
        /// Returns the object at the position.
        /// Returns null if there is no object at the position.
        /// </returns>
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

        /// <summary>
        /// Gets or sets the object at the specified position.
        /// Returns null if there is no object at the position.
        /// </summary>
        /// <param name="x">The x-coordinate of the position</param>
        /// <param name="y">The y-coordinate of the position</param>
        /// <returns>
        /// Returns the object at the position.
        /// Returns null if there is no object at the position.
        /// </returns>
        public T? this[int x, int y]
        {
            get => this[new HexPoint(x, y)];
            set => this[new HexPoint(x, y)] = value;
        }

        /// <summary>
        /// Clears all objects stored.
        /// </summary>
        public void Clear()
        {
            dictonary.Clear();
        }

        #region Interfaces

        /// <summary>
        /// Returns a byte array of the grid.
        /// </summary>
        /// <returns>Returns a byte array of the grid.</returns>
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

        /// <summary>
        /// Loads a grid from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <param name="startIndex">The start index in the array</param>
        /// <returns>Returns the amount of bytes loaded.</returns>
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

        /// <summary>
        /// Returns a copy of the grid.
        /// </summary>
        /// <returns>Returns a copy of the grid.</returns>
        public HexGrid<T> Copy()
        {
            HexGrid<T> copy = new HexGrid<T>();
            copy.FromBytes(ToBytes(), 0);
            return copy;
        }

        /// <summary>
        /// Returns all values.
        /// </summary>
        /// <returns>Returns all values.</returns>
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