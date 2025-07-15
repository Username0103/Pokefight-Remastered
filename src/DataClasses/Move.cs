using MessagePack;

namespace src.DataClasses
{
    [MessagePackObject]
    public record struct Move
    {
        [Key(0)]
        public required string Name;

        [Key(1)]
        // No two-type moves in gen 1.[Key(0)]
        public required Type Type;

        [Key(2)]
        public required Category Category;

        [Key(3)]
        public required int PP;

        [Key(4)]
        public required int? Priority;

        [Key(5)]
        public required int? Power;

        [Key(6)]
        public required int? Accuracy;
    }
}
