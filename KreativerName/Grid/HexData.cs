using System.Collections.Generic;

namespace KreativerName.Grid
{
    public struct HexData
    {
        public int Index;
        public HexFlags Flags;

        public List<HexChange> Changes;
    }

    public struct HexChange
    {
        public int ChangeTo;
        public HexCondition Condition;
    }

    public enum HexCondition
    {
        Move,
        PlayerEnter,
        PlayerLeave
    }
}
