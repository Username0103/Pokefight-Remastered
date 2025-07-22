using Spectre.Console;
using Src.Misc;

namespace Src.UI
{
    public static class MainMenu
    {
        public static Homework DisplayMenu(bool hasLastBattle, AudioPlayer SFXPlayer)
        {
            AnsiConsole.MarkupLine("[bold italic]Main Menu[/]");
            OrderedDictionary<string, Homework> choices = [];
            choices.Add("Play", Homework.CreateNew);
            if (hasLastBattle)
            {
                choices.Add("Replay last battle", Homework.UseExisting);
            }
            choices.Add("Options", Homework.OptionsChanged);
            choices.Add("Exit", Homework.Exit);

            string chosen = AnsiConsole.Prompt(
                new SelectionPrompt<string>().AddChoices([.. choices.Keys])
            );
            SFXPlayer.Play(Sounds.ButtonPress);
            var homework = choices[chosen];
            if (homework == Homework.OptionsChanged)
            {
                OptionsMenu(SFXPlayer);
                return DisplayMenu(hasLastBattle, SFXPlayer);
            }
            else if (homework == Homework.Exit)
            {
                Environment.Exit(0);
            }
            return homework;
        }

        private static void OptionsMenu(AudioPlayer SFXPlayer)
        {
            static float groundValue(float v) =>
                v > 100 ? 100F
                : v < 0 ? 0F
                : v;

            AnsiConsole.MarkupLine("[bold]Options Menu[/]");
            var exitText = "Exit Options Menu";
            var (_, startPosRow) = Console.GetCursorPosition();
            while (true)
            {
                var optionSelections = new Dictionary<string, Action<float>>
                {
                    {
                        $"Music Volume ({(int)(GameOptions.MusicVolume * 100)}%)",
                        (v) =>
                        {
                            GameOptions.MusicVolume = groundValue(v) / 100;
                        }
                    },
                    {
                        $"Effects Volume ({(int)(GameOptions.SFXVolume * 100)}%)",
                        (v) =>
                        {
                            GameOptions.SFXVolume = groundValue(v) / 100;
                        }
                    },
                    { exitText, (_) => { } },
                };
                var optionName = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select option to change")
                        .AddChoices([.. optionSelections.Keys])
                );
                SFXPlayer.Play(Sounds.ButtonPress);
                var (_, endPosRow) = Console.GetCursorPosition();
                Utils.ClearConsoleArea(
                    startX: 0,
                    startY: startPosRow - 1,
                    endX: Console.WindowWidth,
                    endY: endPosRow
                );
                if (optionName == exitText)
                {
                    break;
                }
                var value = AnsiConsole.Ask<float>($"Value for {optionName}: ");
                SFXPlayer.Play(Sounds.ButtonPress);
                optionSelections[optionName](value);
                GameOptions.Save();
            }
        }

        public enum Homework : byte
        {
            CreateNew,
            UseExisting,
            OptionsChanged,
            Exit,
        }
    }
}
