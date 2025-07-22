using MessagePack;

namespace Src.DataClasses
{
    [MessagePackObject]
    public record class PokemonBattleDefinition
    {
        [Key(0)]
        public required PokemonDefinition Player;

        [Key(1)]
        public required PokemonDefinition Enemy;

        [Key(2)]
        public required Move[] PlayerMoves;

        [Key(3)]
        public required Move[] EnemyMoves;

        [Key(4)]
        public required int PlayerLevel;

        [Key(5)]
        public required int EnemyLevel;
    }
}
