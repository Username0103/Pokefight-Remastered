using Src.DataClasses;

namespace Src.Battle
{
    public class Controller(PokemonDefinition player, PokemonDefinition enemy)
    {
        public void Start()
        {
            Console.WriteLine($"Your pokemon: {player.Name}");
            Console.WriteLine($"Enemy pokemon: {enemy.Name}");
        }

        private void Turn()
        {
            Console.WriteLine("Select move: ");
        }
    }
}
