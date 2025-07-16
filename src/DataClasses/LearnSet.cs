using MessagePack;

namespace Src.DataClasses
{
    [MessagePackObject]
    public record class LearnSet
    {
        [Key(0)]
        public required LearnMove[] Moves;

        [MessagePackObject]
        public struct LearnMove
        {
            [Key(0)]
            public required int Level;

            [Key(1)]
            public required Move Move;

            [Key(2)]
            public required bool isNatural;
        }
    }
}
