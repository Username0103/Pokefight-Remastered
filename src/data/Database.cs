using Microsoft.EntityFrameworkCore;
using Src.Data.models;
using static Src.Misc.Utils;

namespace Src.Data
{
    public static class Database
    {
        public static void Initialize()
        {
            string path = GetDbPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? path);
            if (!File.Exists(path) && !File.Exists(Methods.CACHE_PATH))
                ExtractDb(path);
        }

        private static void ExtractDb(string path)
        {
            var DbResourceName = (
                GetResourcesWithEnding(".sqlite")
                ?? throw new SystemException(
                    $"Could not get database from EXE embeds."
                        + " Ensure the PackageReference in .csproj is correct."
                )
            ).Single();

            using Stream resourceStream =
                assembly.GetManifestResourceStream(DbResourceName)
                ?? throw new SystemException($"Could not get database with name {DbResourceName}.");

            using FileStream fileStream = new(
                path: path,
                mode: FileMode.Create,
                access: FileAccess.Write
            );
            resourceStream.CopyTo(fileStream);
        }

        public static string GetDbPath()
        {
            var folder = Environment.SpecialFolder.CommonApplicationData;
            var path = Environment.GetFolderPath(folder);
            return Path.Join(path, "Username0103", "PokeFight-Remastered", "Pokedex.db");
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<Moves> Moves { get; set; }
        public DbSet<Pokemon> Pokemon { get; set; }
        public DbSet<PokemonMoves> PokemonMoves { get; set; }
        public DbSet<PokemonStats> PokemonStats { get; set; }
        public DbSet<PokemonTypes> PokemonTypes { get; set; }
        public DbSet<TypeEffectiveness> TypeEffectiveness { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={Data.Database.GetDbPath()}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            void Add<T>()
                where T : class
            {
                modelBuilder.Entity<T>().HasNoKey();
            }
            Add<Moves>();
            Add<Pokemon>();
            Add<PokemonMoves>();
            Add<PokemonStats>();
            Add<PokemonTypes>();
            Add<TypeEffectiveness>();
        }
    }
}
