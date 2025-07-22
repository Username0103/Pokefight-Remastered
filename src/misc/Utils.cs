using System.Reflection;
using Spectre.Console;
using static System.Environment;

namespace Src.Misc
{
    public static class Utils
    {
        public static readonly Random Generator = new();

        public static readonly string UserDataPath = GetAppDir(SpecialFolder.ApplicationData);
        public static readonly string SystemDataPath = GetAppDir(
            SpecialFolder.CommonApplicationData
        );

        public static readonly string DbPath = Path.Join(SystemDataPath, "Pokedex.db");
        public static readonly string LastBattlePath = Path.Join(UserDataPath, "last_battle.bin");
        public static readonly string OptionsPath = Path.Join(UserDataPath, "chosen_options.bin");

        public static readonly Assembly assembly =
            Assembly.GetExecutingAssembly()
            ?? throw new SystemException("Could not get execution assembly.");

        private static string GetAppDir(SpecialFolder dir)
        {
            var path = Path.Join(GetFolderPath(dir), "Username0103", "PokeFight-Remastered");
            Directory.CreateDirectory(path);
            return path;
        }

        public static string[] GetResourcesWithEnding(string ending)
        {
            var names = assembly.GetManifestResourceNames();
            return [.. names.Where((n) => n.EndsWith(ending))];
        }

        public static void ClearConsoleArea(int startX, int startY, int endX, int endY)
        {
            if (startX > endX)
            {
                (startX, endX) = (endX, startX);
            }
            if (startY > endY)
            {
                (startY, endY) = (endY, startY);
            }

            startX = Math.Max(0, startX);
            startY = Math.Max(0, startY);
            endX = Math.Min(endX, Console.WindowWidth - 1);
            endY = Math.Min(endY, Console.WindowHeight - 1);

            AnsiConsole.Cursor.Show(false);
            foreach (var X in Enumerable.Range(startX, endX - startX + 1))
            {
                foreach (var Y in Enumerable.Range(startY, endY - startY + 1))
                {
                    Console.SetCursorPosition(X, Y);
                    Console.Write(" ");
                }
            }
            Console.SetCursorPosition(startX, startY);
            AnsiConsole.Cursor.Show(true);
        }

        public static string[] Capitalize(string[] strings)
        {
            return
            [
                .. strings.Select(str =>
                {
                    if (string.IsNullOrEmpty(str))
                    {
                        return str;
                    }

                    return new string(
                        [.. str.Select((c, index) => index == 0 ? char.ToUpper(c) : c)]
                    );
                }),
            ];
        }

        public static string Capitalize(string str)
        {
            if (str.Length == 0)
            {
                return str;
            }
            return new string([.. str.Select((c, index) => index == 0 ? char.ToUpper(c) : c)]);
        }
    }
}
