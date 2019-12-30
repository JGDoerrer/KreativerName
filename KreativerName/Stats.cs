using System;
using System.Collections.Generic;
using System.IO;
using KreativerName.UI;

namespace KreativerName
{
    public struct Stats : IBytes
    {
        public static Stats Current = new Stats();
        private int totalMoves;
        private TimeSpan timePlaying;
        private DateTime firstStart;
        private int levelsCompleted;
        private int levelsCompletedPerfect;
        private int fails;
        private int tetrisMostLines;
        private int tetrisHighScore;
        private int tetrisHighLevel;

        public int TotalMoves { get => totalMoves; set => totalMoves = value; }
        public TimeSpan TimePlaying { get => timePlaying; set => timePlaying = value; }
        public DateTime FirstStart { get => firstStart; set => firstStart = value; }
        public int LevelsCompleted { get => levelsCompleted; set => levelsCompleted = value; }
        public int LevelsCompletedPerfect { get => levelsCompletedPerfect; set => levelsCompletedPerfect = value; }
        public int Fails { get => fails; set => fails = value; }
        public int TetrisMostLines { get => tetrisMostLines; set => tetrisMostLines = value; }
        public int TetrisHighScore { get => tetrisHighScore; set => tetrisHighScore = value; }
        public int TetrisHighLevel { get => tetrisHighLevel; set => tetrisHighLevel = value; }

        public byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(TotalMoves.ToBytes());
            bytes.AddRange(TimePlaying.ToBytes());
            bytes.AddRange(FirstStart.ToBytes());
            bytes.AddRange(LevelsCompleted.ToBytes());
            bytes.AddRange(LevelsCompletedPerfect.ToBytes());
            bytes.AddRange(Fails.ToBytes());
            bytes.AddRange(TetrisMostLines.ToBytes());
            bytes.AddRange(TetrisHighScore.ToBytes());
            bytes.AddRange(TetrisHighLevel.ToBytes());

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            count += totalMoves.FromBytes(bytes, startIndex + count);
            count += timePlaying.FromBytes(bytes, startIndex + count);
            count += firstStart.FromBytes(bytes, startIndex + count);
            count += levelsCompleted.FromBytes(bytes, startIndex + count);
            count += levelsCompletedPerfect.FromBytes(bytes, startIndex + count);
            count += fails.FromBytes(bytes, startIndex + count);
            count += tetrisMostLines.FromBytes(bytes, startIndex + count);
            count += tetrisHighScore.FromBytes(bytes, startIndex + count);
            count += tetrisHighLevel.FromBytes(bytes, startIndex + count);

            return count;
        }


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