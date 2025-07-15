using MessagePack;

namespace src.DataClasses
{
    [MessagePackObject]
    public record struct Pokemon
    {
        [Key(0)]
        public required string Name;

        [Key(1)]
        public required Pokestats Stats;

        [Key(2)]
        public required Type Type1;

        [Key(3)]
        public required Type? Type2;

        [Key(4)]
        public required LearnSet learnset;

        [Key(5)]
        public required Move[] moves;
    }
}
