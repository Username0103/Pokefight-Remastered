using Spectre.Console;
using Src.DataClasses;
using Src.Misc;
using static Src.Misc.Sound;
using static Src.Misc.Utils;

namespace Src.Battle
{
    public class BattleController(PokemonBattle battle, AudioPlayer SFX)
    {
        public void Start()
        {
            AnsiConsole.MarkupLine(
                $"Player pokemon: Level [blue]{battle.Player.Level}[/]"
                    + $" [red]{battle.Player.Definition.Name}[/] with {battle.Enemy.Health} HP."
            );
            AnsiConsole.MarkupLine(
                $"Enemy pokemon: Level [blue]{battle.Enemy.Level}[/]"
                    + $" [red]{battle.Enemy.Definition.Name}[/] with {battle.Enemy.Health} HP."
            );
            Delay(2);

            int turnCounter = 1;
            while (true)
            {
                AnsiConsole.MarkupLine($"[italic]Turn {turnCounter}.[/]");
                AdvanceTurn();
                turnCounter++;
            }
        }

        private void AdvanceTurn()
        {
            var MovesDict = battle.Player.Moves.ToDictionary(
                k => $"{k.Move.Name} ({k.PP.max}/{k.Move.PP})",
                v => v
            );
            var selectedMoveName = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("Select move: ").AddChoices([.. MovesDict.Keys])
            );
            SFX.Play(Sounds.ButtonPress);
            var playerMove = MovesDict[selectedMoveName];
            var enemyMove = GetAiMove(battle.Enemy.Moves);
        }

        private static MoveWithPP GetAiMove(MoveWithPP[] moves)
        {
            return moves[Generator.Next(0, moves.Length)];
        }
    }
}
