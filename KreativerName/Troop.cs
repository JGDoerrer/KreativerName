namespace KreativerName
{
    public struct Troop
    {
        public Troop(int team, TroopType type)
        {
            Team = team;
            Type = type;
        }

        public int Team { get; set; }
        public TroopType Type { get; set; }
    }

    public enum TroopType
    {
        Troop1,
    }
}