using Microsoft.EntityFrameworkCore;
using Src.Data.models;
using static Src.Misc.Utils;

namespace Src.Data
{
    public static class Database
    {
        public static void Initialize()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DbPath) ?? DbPath);
            if (!File.Exists(DbPath) && !File.Exists(Methods.CACHE_PATH))
                ExtractDb(DbPath);
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
            options.UseSqlite($"Data Source={DbPath}");
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
