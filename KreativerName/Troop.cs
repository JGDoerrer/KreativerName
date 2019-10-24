using System;

namespace KreativerName
{
    public struct Troop : IBytes
    {
        public Troop(int team, TroopType type)
        {
            Team = team;
            Type = type;
        }

        public int Team { get; set; }
        public TroopType Type { get; set; }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[5];
            BitConverter.GetBytes(Team).CopyTo(bytes, 0);
            bytes[4] = (byte)Type;
            return bytes;
        }

        public void FromBytes(byte[] bytes, int startIndex)
        {
            Team = BitConverter.ToInt32(bytes, startIndex);
            Type = (TroopType)bytes[startIndex + 4];
        }
    }

    public enum TroopType : byte
    {
        Pawn,
        Knight,
        Rook
    }
}