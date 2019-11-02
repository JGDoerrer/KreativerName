using System;
using System.Collections.Generic;
using System.Linq;
using KreativerName;
using KreativerName.Grid;

namespace Test
{
    class Program
    {
        static Game game;
        static bool done;

        static void Main(string[] args)
        {
            game = new Game();
            game.loadNextLevel = false;
            game.LoadWorld(0);

            for (int i = 0; i < game.World.levels.Count; i++)
            {
                done = false;
                game.LoadLevel(i);
                var result = DoMoves(game.GetPlayerMoves().Select(x => new List<HexPoint>() { x }).ToList());

                Console.Write($"Level {i + 1}: ");
                foreach (var move in result)
                {
                    Console.Write(move);
                }
                Console.WriteLine();
            }

            Console.WriteLine("Done");
            Console.ReadKey(true);
        }

        static List<HexPoint> DoMoves(List<List<HexPoint>> moves)
        {
            if (done)
                return null;

            HashSet<List<HexPoint>> nextMoves = new HashSet<List<HexPoint>>();
            int count = moves.Count;

            for (int i = moves.Count - 1; i >= 0; i--)
            {
                List<HexPoint> move = moves[i];
                moves.RemoveAt(i);
                if (done)
                    break;

                game.player = move.Last();

                if (game.Grid[game.player].Value.Type == HexType.Goal)
                {
                    done = true;
                    return move;
                }
                else if (game.Grid[game.player].Value.Type == HexType.Deadly)
                {
                    continue;
                }

                foreach (var m in game.GetPlayerMoves())
                {
                    if (moves.All(x => !x.Contains(m)))
                        nextMoves.Add(move.Append(m).ToList());
                }

                if (i % 100 == 0)
                {
                    Console.Write($"{count - i,6}/{count - 1,6} {100f * (count - i) / (count - 1),3:N0}%");
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            }

            moves.Clear();

            Console.WriteLine($"Move done: {nextMoves.Count} next moves");
            var result = DoMoves(nextMoves.ToList());

            if (result != null)
            {
                return result;
            }
            return null;
        }
    }
}
