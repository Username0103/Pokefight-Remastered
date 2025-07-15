using System.Text.Json;
using src.data;

namespace src
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Database.Initialize();
            var options = new JsonSerializerOptions { IncludeFields = true, WriteIndented = true };
            Console.WriteLine(JsonSerializer.Serialize(Methods.GetAllPokemon()[149], options));
        }
    }
}
