using System.Reflection;
using Microsoft.EntityFrameworkCore;
using src.data.models;

namespace src.data
{
    public static class Database
    {
        public static void Initialize()
        {
            string path = GetDbPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? path);
            if (!File.Exists(path))
                ExtractDb(path);
        }

        private static void ExtractDb(string path)
        {
            Assembly assembly =
                Assembly.GetExecutingAssembly()
                ?? throw new SystemException("Could not get execution assembly.");

            var DbResourceName =
                GetDbResourceName(assembly)
                ?? throw new SystemException(
                    $"Could not get database from EXE embeds."
                        + " Ensure the PackageReference in .csproj is correct."
                );

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

        private static string? GetDbResourceName(Assembly assembly)
        {
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                if (name.EndsWith(".sqlite"))
                {
                    return name;
                }
            }
            return null;
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

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={data.Database.GetDbPath()}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Moves>().HasNoKey();
            modelBuilder.Entity<Pokemon>().HasNoKey();
            modelBuilder.Entity<PokemonMoves>().HasNoKey();
            modelBuilder.Entity<PokemonStats>().HasNoKey();
            modelBuilder.Entity<PokemonTypes>().HasNoKey();
        }
    }
}
