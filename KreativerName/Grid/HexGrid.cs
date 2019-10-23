using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class HexGrid<T> : IEnumerable<T> where T : struct
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
    }
}