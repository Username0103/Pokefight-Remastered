using MessagePack;

namespace Src.DataClasses
{
    [MessagePackObject]
    public record class Pokestats
    {
        [Key(0)]
        public required int HP;

        [Key(1)]
        public required int Attack;

        [Key(2)]
        public required int Defense;

        [Key(3)]
        public required int SpAttack;

        [Key(4)]
        public required int SpDefense;

        [Key(5)]
        public required int Speed;
    }
}
