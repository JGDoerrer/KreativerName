using System.Drawing;

namespace KreativerName.Grid
{
    public struct Player
    {
        public Player(Color color)
        {
            Position = new HexPoint();
            LastPosition = new HexPoint();
            Color = color;
            IsDead = false;
        }
        public Player(HexPoint position, Color color)
        {
            Position = position;
            LastPosition = position;
            Color = color;
            IsDead = false;
        }

        public HexPoint Position;
        public HexPoint LastPosition;
        public Color Color;
        public bool IsDead;
    }
}
