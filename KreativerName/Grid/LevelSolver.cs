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

        public event EmptyEvent Solved;
        public int MinMoves { get; private set; }
        public List<HexPoint> Solution { get; private set; }

        public void Solve()
        {
            List<List<HexPoint>> results = new List<List<HexPoint>>();
            int moves = 1;

            while (results.Count == 0)
            {
                results = Solve(level.GetPossibleMoves(level.startPos), level.Copy(), moves, new List<HexPoint>());
                moves++;
            }

            MinMoves = results.Min(x => x.Count);
            Solution = results.Where(x => x.Count == MinMoves).First();
            Solved?.Invoke();
        }

        public void SolveAsync(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<List<HexPoint>> results = new List<List<HexPoint>>();
            int moves = 1;
            bool done = false;

            while (!done)
            {
                results = Solve(level.GetPossibleMoves(level.startPos), level.Copy(), moves, new List<HexPoint>());

                if (results.Count > 0)
                {
                    List<List<HexPoint>> prevResults = results;

                    moves--;
                    results = Solve(level.GetPossibleMoves(level.startPos), level.Copy(), moves, new List<HexPoint>());

                    if (results.Count == 0)
                        results = prevResults;
                        done = true;
                }

                moves +=2;
                worker.ReportProgress(moves);
            }
            
            MinMoves = results.Min(x => x.Count);
            Solution = results.Where(x => x.Count == MinMoves).First();
            worker.ReportProgress(100);
            Solved?.Invoke();
        }

        private List<List<HexPoint>> Solve(List<HexPoint> moves, Level level, int movesLeft, List<HexPoint> prevMoves)
        {
            List<List<HexPoint>> results = new List<List<HexPoint>>();

            if (movesLeft == 0)
                return null;

            foreach (var move in moves)
            {
                Level copy = level;//.Copy();
                copy.Update(move);

                HexFlags flags = copy.grid[move].Value.Flags;

                if (flags.HasFlag(HexFlags.Deadly))
                    continue;

                if (flags.HasFlag(HexFlags.Goal))
                    results.Add(prevMoves.Append(move).ToList());
                
                List<List<HexPoint>> result = Solve(copy.GetPossibleMoves(move), copy, movesLeft - 1, prevMoves.Append(move).ToList());
                if (result != null)
                    results.AddRange(result);
            }

            return results;
        }
    }
}
