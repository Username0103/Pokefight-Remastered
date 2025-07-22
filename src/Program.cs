using MessagePack;
using Spectre.Console;
using Src.Data;
using Src.DataClasses;
using Src.Misc;
using Src.UI;
using static Src.UI.MainMenu;

namespace Src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Database.Initialize();
            AnsiConsole.MarkupLine("[bold italic]Pokefight-Remastered[/]");
            GameOptions.Load();
            var pokemon = Methods.GetAllPokemon(out var effectivenesses);
            var songPlayer = new AudioPlayer(shouldLoop: true, isMusic: true);
            var SFXPlayer = new AudioPlayer(shouldLoop: false, isMusic: false);
            songPlayer.Play(Sounds.TitleScreenSong);
            Console.WriteLine(GetPokemon(pokemon, SFXPlayer));
        }

        private static PokemonBattle GetPokemon(
            PokemonDefinition[] definitions,
            AudioPlayer effectsPlayer
        )
        {
            var homework = DisplayMenu(
                hasLastBattle: File.Exists(Utils.LastBattlePath),
                SFXPlayer: effectsPlayer
            );
            if (homework == Homework.CreateNew)
            {
                StartingPokemonGet.GetStartingPokemon(
                    definitions,
                    out var player,
                    out var enemy,
                    effectsPlayer
                );
                var battleDefinition = new PokemonBattleDefinition()
                {
                    Enemy = enemy.Definition,
                    Player = player.Definition,
                    EnemyLevel = enemy.Level,
                    PlayerLevel = player.Level,
                    EnemyMoves = [.. enemy.Moves.Select((m) => m.Move)],
                    PlayerMoves = [.. player.Moves.Select((m) => m.Move)],
                };
                var battle = new PokemonBattle(player, enemy);
                File.WriteAllBytes(
                    Utils.LastBattlePath,
                    MessagePackSerializer.Serialize(battleDefinition)
                );
                return battle;
            }
            else if (homework == Homework.UseExisting)
            {
                var definition = MessagePackSerializer.Deserialize<PokemonBattleDefinition>(
                    File.ReadAllBytes(Utils.LastBattlePath)
                );
                return new(
                    new Pokemon(definition.Player, definition.PlayerMoves, definition.PlayerLevel),
                    new Pokemon(definition.Enemy, definition.EnemyMoves, definition.EnemyLevel)
                );
            }
            else
            {
                throw new NotImplementedException($"unrecognized case Homework.{homework}");
            }
        }
    }
}
