namespace Src.DataClasses
{
    public record class PokemonBattle
    {
        public readonly Pokemon Player;

        public readonly Pokemon Enemy;

        public PokemonBattle(Pokemon player, Pokemon enemy)
        {
            Player = player;
            Enemy = enemy;
        }
    }
}
