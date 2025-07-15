namespace src.DataClasses
{
    public enum Type : byte
    {
        // these correspond the the ids on the DB types table
        Normal = 1,
        Fighting = 2,
        Flying = 3,
        Poison = 4,
        Ground = 5,
        Rock = 6,
        Bug = 7,
        Ghost = 8,

        // skip steel since it's gen 2
        Fire = 10,
        Water = 11,
        Grass = 12,
        Electric = 13,
        Psychic = 14,
        Ice = 15,
        Dragon = 16,
    }
}
