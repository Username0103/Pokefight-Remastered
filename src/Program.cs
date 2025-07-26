using MessagePack;
using Spectre.Console;
using Src.Battle;
using Src.Data;
using Src.DataClasses;
using Src.Misc;
using Src.UI;
using static Src.Misc.Sound;
using static Src.UI.MainMenu;

namespace Src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Database.Initialize();
            DisplayTitle();
            Load(out var pokemon, out var effectivenesses);
            GameOptions.Load();
            var songPlayer = new AudioPlayer(isMusic: true);
            var SFXPlayer = new AudioPlayer(isMusic: false);
            songPlayer.Play(Sounds.TitleScreenSong);
            var battleSetup = SetupBattle(pokemon, SFXPlayer);
            songPlayer.Play(Sounds.BattleSong);
            new BattleController(battleSetup, SFXPlayer, effectivenesses).Start();
        }

        private static void Load(
            out PokemonDefinition[] pokemon,
            out Effectiveness[] effectivenesses
        )
        {
            (effectivenesses, pokemon) = AnsiConsole
                .Status()
                .Spinner(Spinner.Known.Line)
                .Start(
                    "Loading...",
                    (c) =>
                    {
                        Methods.LoadAllData(out var effectivenesses, out var pokemon);
                        return (effectivenesses, pokemon);
                    }
                );
        }

        private static void DisplayTitle()
        {
            AnsiConsole.MarkupLine("[bold italic]Pokéfight-Remastered[/]");
        }

        private static PokemonBattle SetupBattle(
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
