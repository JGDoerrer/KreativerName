using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName
{
    public class Stats
    {
        //TODO: Save & Load

        public int TotalMoves { get; set; }
        public TimeSpan TimePlaying { get; set; }
        public int LevelsCompleted { get; set; }
        public int LevelsCompletedPerfect { get; set; }


        public static Stats Current { get; set; } = new Stats();
    }
}