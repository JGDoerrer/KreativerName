using System;
using System.Collections.Generic;
using System.IO;

namespace KreativerName
{
    public struct Stats : IBytes
    {
        public int TotalMoves;
        public TimeSpan TimePlaying;
        public DateTime FirstStart;
        public int LevelsCompleted;
        public int LevelsCompletedPerfect;
        public int Deaths;
        public int TetrisMostLines;
        public int TetrisHighScore;
        public int TetrisHighLevel;

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(TotalMoves.ToBytes());
            bytes.AddRange(TimePlaying.ToBytes());
            bytes.AddRange(FirstStart.ToBytes());
            bytes.AddRange(LevelsCompleted.ToBytes());
            bytes.AddRange(LevelsCompletedPerfect.ToBytes());
            bytes.AddRange(Deaths.ToBytes());
            bytes.AddRange(TetrisMostLines.ToBytes());
            bytes.AddRange(TetrisHighScore.ToBytes());
            bytes.AddRange(TetrisHighLevel.ToBytes());

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            count += TotalMoves.FromBytes(bytes, startIndex + count);
            count += TimePlaying.FromBytes(bytes, startIndex + count);
            count += FirstStart.FromBytes(bytes, startIndex + count);
            count += LevelsCompleted.FromBytes(bytes, startIndex + count);
            count += LevelsCompletedPerfect.FromBytes(bytes, startIndex + count);
            count += Deaths.FromBytes(bytes, startIndex + count);
            count += TetrisMostLines.FromBytes(bytes, startIndex + count);
            count += TetrisHighScore.FromBytes(bytes, startIndex + count);
            count += TetrisHighLevel.FromBytes(bytes, startIndex + count);

            return count;
        }

        public static Stats Current = new Stats();

        public void SaveToFile(string name)
        {
            string path = $@"Resources\{name}.sts";

            byte[] bytes = ToBytes();
            File.WriteAllBytes(path, bytes);
        }

        public static Stats LoadFromFile(string name)
        {
            Stats stats = new Stats();
            string path = $@"Resources\{name}.sts";

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                stats.FromBytes(bytes, 0);
            }

            return stats;
        }
    }
}