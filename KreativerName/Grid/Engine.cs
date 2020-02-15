using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KreativerName.Grid
{
    /// <summary>
    /// A class for the game logic.
    /// </summary>
    public class Engine
    {
        public Engine()
        { }
        public Engine(Level level)
        {
            Level = level;
            Players = new Player[1]
            {   
                new Player(level.StartPos, Color.FromArgb(255, 0, 255, 0)),
            };
        }

        public Level Level;
        public Player[] Players;
        public bool LevelDone => Players.All(x => Level.Grid[x.Position].Value.GetFlags(Level.Data).HasFlag(HexFlags.Goal));

        /// <summary>
        /// Updates the level.
        /// </summary>
        /// <param name="player">The index of the player.</param>
        /// <param name="pos">The new position of the player.</param>
        public void Update(int player, HexPoint pos)
        {
            // Update Positions
            Players[player].LastPosition = Players[player].Position;
            Players[player].Position = pos;

            HexGrid<Hex> nextGrid = Level.Grid.Copy();

            for (int i = 0; i < Level.Grid.Count; i++)
            {
                Hex hex = Level.Grid.Values.ElementAt(i);

                UpdateHex(hex, player, ref nextGrid);
            }

            Level.Grid = nextGrid;
            
            HexFlags flags = Level.Grid[Players[player].Position].Value.GetFlags(Level.Data);

            // If player is on a deadly or solid tile, reset
            if (flags.HasFlag(HexFlags.Deadly) || flags.HasFlag(HexFlags.Solid))
            {
                Players[player].IsDead = true;
            }

            if (flags.HasFlag(HexFlags.Goal))
            {
            }
        }

        private void UpdateHex(Hex hex, int player, ref HexGrid<Hex> nextGrid)
        {
            foreach (HexData type in hex.GetTypes(Level.Data))
            {
                HexPoint position = hex.Position;

                foreach (HexAction action in type.Changes)
                {
                    bool conditionMet = false;
                    bool moveHex = action.Flags.HasFlag(HexActionFlags.MoveHex);
                    HexPoint nextPos = position + new HexPoint(action.MoveX, action.MoveY);

                    conditionMet = CheckCondition(player, position, action, nextPos);

                    if (conditionMet && nextGrid[position].Value.IDs.Contains(type.ID))
                    {
                        if (nextPos == position || !moveHex)
                        {
                            Hex temp = nextGrid[position].Value;
                            temp.IDs.Remove(type.ID);
                            temp.IDs.Add(action.ChangeTo);
                            nextGrid[position] = temp;
                        }
                        else if (nextGrid[nextPos].HasValue && moveHex)
                        {
                            Hex temp = nextGrid[position].Value;
                            temp.IDs.Remove(type.ID);

                            Hex next = nextGrid[nextPos].Value;
                            next.IDs.Add(action.ChangeTo);

                            nextGrid[position] = temp;
                            nextGrid[nextPos] = next;

                            position = nextPos;
                        }
                    }
                }
            }
        }

        private bool CheckCondition(int player, HexPoint position, HexAction action, HexPoint nextPos)
        {
            bool conditionMet = false;

            switch (action.Condition)
            {
                case HexCondition.Move:
                    conditionMet = true;
                    break;
                case HexCondition.PlayerEnter:
                    conditionMet = position == Players[player].Position;
                    break;
                case HexCondition.PlayerLeave:
                    conditionMet = position == Players[player].LastPosition;
                    break;
                case HexCondition.NextFlag:
                    conditionMet = Level.Grid[nextPos] == null || (Level.Grid[nextPos]?.GetFlags(Level.Data) & (HexFlags)action.Data) != 0;
                    break;
                case HexCondition.NextNotFlag:
                    conditionMet = Level.Grid[nextPos] != null && (Level.Grid[nextPos]?.GetFlags(Level.Data) & (HexFlags)action.Data) == 0;
                    break;
                case HexCondition.NextID:
                    conditionMet = Level.Grid[nextPos] == null || Level.Grid[nextPos]?.IDs.Contains(action.Data) == true;
                    break;
                case HexCondition.NextNotID:
                    conditionMet = Level.Grid[nextPos] != null && Level.Grid[nextPos]?.IDs.Contains(action.Data) != true;
                    break;
                case HexCondition.PlayerEnterID:
                    conditionMet = Level.Grid[Players[player].Position]?.IDs.Contains(action.Data) == true;
                    break;
                case HexCondition.PlayerLeaveID:
                    conditionMet = Level.Grid[Players[player].LastPosition]?.IDs.Contains(action.Data) == true;
                    break;
            }

            return conditionMet;
        }

        /// <summary>
        /// Returns all possible moves for a player position.
        /// </summary>
        /// <param name="player">The position of the player.</param>
        /// <returns>All possible moves for the specified player position.</returns>
        public List<HexPoint> GetPossibleMoves(HexPoint player)
        {
            HexPoint[] directions = {
                new HexPoint( 1,  0),
                new HexPoint( 1, -1),
                new HexPoint( 0, -1),
                new HexPoint(-1,  0),
                new HexPoint(-1,  1),
                new HexPoint( 0,  1),
            };

            if (Level.Grid == null)
                return new List<HexPoint>();

            List<HexPoint> moves = new List<HexPoint>();

            for (int i = 0; i < 6; i++)
            {
                int j = 1;

                while (Level.Grid[(directions[i] * j) + player].HasValue && !Level.Grid[(directions[i] * j) + player].Value.GetFlags(Level.Data).HasFlag(HexFlags.Solid))
                {
                    moves.Add(directions[i] * j + player);
                    j++;
                }
            }

            return moves;
        }

    }
}
