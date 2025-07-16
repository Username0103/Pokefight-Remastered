using System.Text.Json;
using Src.Data;
using Src.Misc;

namespace Src
{
    public class Program
    {
        private static readonly JsonSerializerOptions options = new()
        {
            IncludeFields = true,
            WriteIndented = true,
            IndentSize = 2,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static void Main(string[] args)
        {
            var player = new AudioPlayer();
            player.Play(Song.TitleScreen);
            Console.ReadKey();
        }

        private static void PrintAllPokemon()
        {
            Database.Initialize();
            var pokemon = Methods.GetAllPokemon(out var effectivenesses);
            Console.WriteLine($"Found: {string.Join(", ", pokemon.Select((p) => p.Name))}.");
        }

        private static void PrintAllData()
        {
            Database.Initialize();
            Console.WriteLine("{\nPokemon\": ");
            Console.Write(
                JsonSerializer.Serialize(Methods.GetAllPokemon(out var effectivenesses), options)
            );
            Console.WriteLine(",\n\t\"Effectivenesses\":");
            Console.WriteLine(JsonSerializer.Serialize(effectivenesses, options));
            Console.Write("}");
        }
    }
}
