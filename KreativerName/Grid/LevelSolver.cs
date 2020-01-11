using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KreativerName.Scenes;

namespace KreativerName.Grid
{
    class LevelSolver
    {
        public LevelSolver(Level level)
        {
            this.level = level.Copy();
        }

        Level level;
        int[] possibleMoves;
        bool logMoves = false;

        public event EmptyEvent Solved;
        public int MinMoves { get; private set; }
        public List<List<HexPoint>> Solutions { get; private set; }

        public void Solve()
        {
            List<List<HexPoint>> results = new List<List<HexPoint>>();
            int moves = 1;

            while (results.Count == 0)
            {
                results = Solve(level.GetPossibleMoves(level.StartPos), level.Copy(), moves, new List<HexPoint>());
                moves++;
            }

            MinMoves = results.Min(x => x.Count);
            Solutions = results.Where(x => x.Count == MinMoves).ToList();
            Solved?.Invoke();
        }

        public void SolveAsync(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<List<HexPoint>> results = new List<List<HexPoint>>();
            int moves = 1;
            bool done = false;

            logMoves = false;
            while (!done)
            {
                logMoves = true;
                possibleMoves = new int[moves];
                results = Solve(level.GetPossibleMoves(level.StartPos), level.Copy(), moves, new List<HexPoint>());

                if (results.Count > 0)
                {
                    List<List<HexPoint>> prevResults = results;

                    if (moves > 1)
                    {
                        moves--;
                        logMoves = true;
                        possibleMoves = new int[moves];

                        results = Solve(level.GetPossibleMoves(level.StartPos), level.Copy(), moves, new List<HexPoint>());

                        if (results.Count == 0)
                            results = prevResults;
                        done = true;
                    }
                }

                for (int i = 1; i < possibleMoves.Length; i++)
                {
                    System.Console.WriteLine($"Possible Moves after {i} moves: {possibleMoves[possibleMoves.Length - i - 1]}");
                }

                moves += 2;
                worker.ReportProgress(moves);
            }


            MinMoves = results.Min(x => x.Count);
            Solutions = results.Where(x => x.Count == MinMoves).ToList();
            worker.ReportProgress(100);
            Solved?.Invoke();
        }

        private List<List<HexPoint>> Solve(List<HexPoint> moves, Level level, int movesLeft, List<HexPoint> prevMoves)
        {
            List<List<HexPoint>> results = new List<List<HexPoint>>();

            if (logMoves)
                possibleMoves[movesLeft - 1]++;

            foreach (var move in moves)
            {
                Level copy = level;//.Copy();
                copy.Update(move);

                HexFlags flags = copy.Grid[move].Value.Flags;

                if (flags.HasFlag(HexFlags.Deadly))
                    continue;

                if (flags.HasFlag(HexFlags.Goal))
                    results.Add(prevMoves.Append(move).ToList());

                if (movesLeft == 1)
                    continue;

                List<List<HexPoint>> result = Solve(copy.GetPossibleMoves(move), copy, movesLeft - 1, prevMoves.Append(move).ToList());
                if (result != null)
                    results.AddRange(result);
            }

            return results;
        }
    }
}
