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
        public static HexFlags GetFlags(this HexType type)
        {
            switch (type)
            {
                case HexType.Normal:
                    return 0;
                case HexType.Solid:
                    return HexFlags.Solid;
                case HexType.Deadly:
                    return HexFlags.Deadly;
                case HexType.Goal:
                    return HexFlags.Goal;
                case HexType.DeadlyTwoStateOn:
                    return HexFlags.Deadly;
                case HexType.DeadlyTwoStateOff:
                    return 0;
                case HexType.DeadlyOneUseOn:
                    return HexFlags.Deadly;
                case HexType.DeadlyOneUseOff:
                    return 0;

                default:
                    return 0;
            }
        }

        public static byte[] ToBytes(this int i)
            => BitConverter.GetBytes(i);
    }
}
