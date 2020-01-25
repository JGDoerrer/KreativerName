using System;
using System.Collections.Generic;
using System.IO;

namespace KreativerName
{
    public struct Stats : IBytes
    {
        public static Stats Current = new Stats();
        private uint totalMoves;
        private TimeSpan timePlaying;
        private DateTime firstStart;
        private uint levelsCompleted;
        private uint levelsCompletedPerfect;
        private uint fails;
        private uint tetrisMostLines;
        private uint tetrisHighScore;
        private uint tetrisHighLevel;
        private uint minesweeperWon;
        private uint minesweeperLost;
        private DateTime lastUpdated;

        public uint TotalMoves { get => totalMoves; set => totalMoves = value; }
        public TimeSpan TimePlaying { get => timePlaying; set => timePlaying = value; }
        public DateTime FirstStart { get => firstStart; set => firstStart = value; }
        public uint LevelsCompleted { get => levelsCompleted; set => levelsCompleted = value; }
        public uint LevelsCompletedPerfect { get => levelsCompletedPerfect; set => levelsCompletedPerfect = value; }
        public uint Fails { get => fails; set => fails = value; }
        public uint TetrisMostLines { get => tetrisMostLines; set => tetrisMostLines = value; }
        public uint TetrisHighScore { get => tetrisHighScore; set => tetrisHighScore = value; }
        public uint TetrisHighLevel { get => tetrisHighLevel; set => tetrisHighLevel = value; }
        public uint MinesweeperWon { get => minesweeperWon; set => minesweeperWon = value; }
        public uint MinesweeperLost { get => minesweeperLost; set => minesweeperLost = value; }
        public DateTime LastUpdated { get => lastUpdated; set => lastUpdated = value; }

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
            bytes.AddRange(MinesweeperWon.ToBytes());
            bytes.AddRange(MinesweeperLost.ToBytes());
            bytes.AddRange(LastUpdated.ToBytes());

            return bytes.ToArray();
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            int count = 0;

            try
            {
                count += totalMoves.FromBytes(bytes, startIndex + count);
                count += timePlaying.FromBytes(bytes, startIndex + count);
                count += firstStart.FromBytes(bytes, startIndex + count);
                count += levelsCompleted.FromBytes(bytes, startIndex + count);
                count += levelsCompletedPerfect.FromBytes(bytes, startIndex + count);
                count += fails.FromBytes(bytes, startIndex + count);
                count += tetrisMostLines.FromBytes(bytes, startIndex + count);
                count += tetrisHighScore.FromBytes(bytes, startIndex + count);
                count += tetrisHighLevel.FromBytes(bytes, startIndex + count);
                count += minesweeperWon.FromBytes(bytes, startIndex + count);
                count += minesweeperLost.FromBytes(bytes, startIndex + count);
                count += lastUpdated.FromBytes(bytes, startIndex + count);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Stats]: Error while loading: {e.Message}");
            }

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