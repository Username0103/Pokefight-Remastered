using Spectre.Console;
using Src.DataClasses;
using Src.Misc;

namespace Src.UI
{
    public static class PokemonSelect
    {
        public static Pokemon SelectPokemon(PokemonDefinition[] definitions, AudioPlayer SFXPlayer)
        {
            // sort pokémon
            definitions = [.. definitions.OrderBy((m) => m.Name)];
            var pokemonDict = definitions.ToDictionary(p => p.Name, p => p);
            var selectedPokemon = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Choose Pokemon:[/]")
                    .AddChoices([.. pokemonDict.Keys])
                    .EnableSearch()
                    .HighlightStyle(new Style(Color.Aqua))
            );
            SFXPlayer.Play(Sounds.ButtonPress);
            AnsiConsole.Markup($"Selected [bold red]{selectedPokemon}[/].\n");
            var selected = pokemonDict[selectedPokemon];
            var level = AnsiConsole.Prompt(new TextPrompt<int>("[aqua]Select your level: [/]"));
            SFXPlayer.Play(Sounds.ButtonPress);
            var isManuallySelecting = AnsiConsole.Prompt(
                new ConfirmationPrompt("[red]Manually select Pokémon moves?[/]")
            );
            SFXPlayer.Play(Sounds.ButtonPress);
            var moves = isManuallySelecting ? SelectMoves(selected, level, SFXPlayer) : null;
            var pokemon = new Pokemon(definition: selected, moves: moves, level: level);
            if (pokemon.Moves.Length == 0)
            {
                AnsiConsole.MarkupLine("[red]No avaliable pokemon moves. Please retry.[/]");
                return SelectPokemon(definitions, SFXPlayer);
            }
            return pokemon;
        }

        private static Move[] SelectMoves(
            PokemonDefinition definition,
            int level,
            AudioPlayer SFXPlayer
        )
        {
            var availableMoves = definition
                .Learnset.Moves.Where((m) => m.Level <= level)
                .OrderBy((m) => m.Move.Name)
                .ToList();
            if (
                AnsiConsole.Prompt(
                    new ConfirmationPrompt("[aqua]Inlude only moves learned by level-up?[/]")
                )
            )
            {
                availableMoves = [.. availableMoves.Where((m) => m.isNatural)];
            }
            SFXPlayer.Play(Sounds.ButtonPress);
            if (availableMoves.Count <= 4)
            {
                AnsiConsole.MarkupLine("[aqua]Automatically selected all avaliable moves.[/]");
                return [.. availableMoves.Select((m) => m.Move)];
            }
            if (availableMoves.Count <= 0)
            {
                return [];
            }
            // remove duplicates due to different learn methods
            availableMoves = [.. availableMoves.DistinctBy((m) => m.Move.Name)];
            var movesDict = availableMoves.ToDictionary(m => m.Move.Name, m => m);
            var countLimited = availableMoves.Count <= 4 ? availableMoves.Count : 4;
            var moves = new Move[countLimited];
            for (int i = 0; i < countLimited; i++)
            {
                var moveName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[red]Select move.[/]")
                        .AddChoices(Utils.Capitalize([.. movesDict.Keys]))
                        .EnableSearch()
                        .HighlightStyle(new Style(Color.Aqua))
                );
                SFXPlayer.Play(Sounds.ButtonPress);
                moveName = moveName.ToLower();
                var moveObj = movesDict[moveName];
                var wasRemoved = movesDict.Remove(moveName);
                if (!wasRemoved)
                {
                    throw new KeyNotFoundException(
                        $"Tried to remove the selected move, but movesDict did not contain element {moveName}.\n\n"
                            + $"movesDict dump: {string.Join("\n", movesDict.Select((m) => $"{m.Key}: {m.Value}"))}"
                    );
                }
                moves[i] = moveObj.Move;
            }
            return moves;
        }
    }
}
