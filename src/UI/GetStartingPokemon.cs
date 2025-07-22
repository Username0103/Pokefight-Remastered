using System.Data;
using Spectre.Console;
using Src.DataClasses;
using Src.Misc;
using static Src.Misc.Sound;

namespace Src.UI
{
    public static class StartingPokemonGet
    {
        private static int level;

        public static void GetStartingPokemon(
            PokemonDefinition[] pokemonDefinitions,
            out Pokemon player,
            out Pokemon enemy,
            AudioPlayer SFXPlayer
        )
        {
            level = Utils.Generator.Next(5, 101);
            AnsiConsole.Markup("[bold aqua]Hello there! Welcome to the world of Pokémon![/]\n");

            player = AskForSelection(pokemonDefinitions, isAiSelection: false, SFXPlayer);
            enemy = AskForSelection(pokemonDefinitions, isAiSelection: true, SFXPlayer);
        }

        private static Pokemon AskForSelection(
            PokemonDefinition[] pokemonDefinitions,
            bool isAiSelection,
            AudioPlayer SFXPlayer
        )
        {
            var role = isAiSelection ? "enemy" : "player";
            var isSelecting = AnsiConsole.Confirm($"[red]Select {role} Pokemon?[/]");
            SFXPlayer.Play(Sounds.ButtonPress);
            if (isSelecting)
            {
                var pokemon = PokemonSelect.SelectPokemon(pokemonDefinitions, SFXPlayer);
                level = pokemon.Level;
                return pokemon;
            }
            else
            {
                var index = Utils.Generator.Next(0, pokemonDefinitions.Length);
                int i = 0;
                while (i < 100)
                {
                    var pokemon = new Pokemon(pokemonDefinitions[index], null, level);
                    if (pokemon.Moves.Length > 0)
                    {
                        return pokemon;
                    }
                    i++;
                }
                throw new DataException($"Could not generate a valid pokemon at level {level}.");
            }
        }
    }
}
