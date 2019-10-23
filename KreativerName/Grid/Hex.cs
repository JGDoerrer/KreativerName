using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Grid
{
    public struct Hex
    {
        public Hex(int x, int y)
        {
            Position = new HexPoint(x, y);
            Troop = null;
        }

        public Hex(HexPoint pos)
        {
            Position = pos;
            Troop = null;
        }

        public Hex(int x, int y, Troop troop)
        {
            Position = new HexPoint(x, y);
            Troop = troop;
        }

        public Hex(HexPoint pos, Troop troop)
        {
            Position = pos;
            Troop = troop;
        }

        public HexPoint Position { get; set; }
        public int X => Position.X;
        public int Y => Position.Y;

        public Troop? Troop { get; set; }
    }
}
