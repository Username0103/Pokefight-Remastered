using MessagePack;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Src.DataClasses;
using Src.Misc;
using static Src.DataClasses.LearnSet;

namespace Src.Data
{
    public static class Methods
    {
        private static readonly int LEVEL_UP_MOVE_LEARN_METHOD = 1;
        public static readonly string CACHE_PATH = Path.Join(
            Path.GetDirectoryName(Database.GetDbPath()),
            "DB_cache.bin"
        );

        [MessagePackObject]
        public struct Cache
        {
            [Key(0)]
            public required PokemonDefinition[] pokemon;

            [Key(1)]
            public required Effectiveness[] effectivenesses;
        }

        // todo: make caching part of the build process rather than running at runtime (to reduce exe size and loading times.)
        // maybe MSBuild?
        public static PokemonDefinition[] GetAllPokemon(out Effectiveness[] typeChart)
        {
            string dbPath = Database.GetDbPath();
            if (File.Exists(CACHE_PATH))
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
                var cached = MessagePackSerializer.Deserialize<Cache>(
                    File.ReadAllBytes(CACHE_PATH)
                );
                typeChart = cached.effectivenesses;
                return cached.pokemon;
            }
            using var db = new DatabaseContext();
            PokemonDefinition[] result =
            [
                .. db
                    .Pokemon.Where(p => p.generation == 1 && p.is_default)
                    .Select(pokemon => BuildPokemon(db, pokemon)),
            ];
            typeChart = GetTypeChart(db);
            var newCache = new Cache { pokemon = result, effectivenesses = typeChart };
            // fire and forget
            Task.Run(async () =>
            {
                await using var stream = new FileStream(path: CACHE_PATH, FileMode.OpenOrCreate);
                await MessagePackSerializer.SerializeAsync(stream, newCache);
                await stream.FlushAsync();
                File.Delete(dbPath);
            });

            return result;
        }

        private static Effectiveness[] GetTypeChart(DatabaseContext db)
        {
            HashSet<int> typeIds = [.. Enum.GetValues<DataClasses.Type>().Select((v) => (int)v)];
            return
            [
                .. from e in db.TypeEffectiveness
                where typeIds.Contains(e.damage_type_id) && typeIds.Contains(e.target_type_id)
                select new Effectiveness
                {
                    Factor = (EffectivenessLevel)e.damage_factor_percent,
                    Source = (DataClasses.Type)e.damage_type_id,
                    Target = (DataClasses.Type)e.target_type_id,
                },
            ];
        }

        private static PokemonDefinition BuildPokemon(DatabaseContext db, models.Pokemon pokemon)
        {
            GetStats(db, pokemon, out Pokestats stats);
            var learnset = GetLearnset(db, pokemon);
            GetPokemonTypes(
                db,
                pokemon,
                out DataClasses.Type type1obj,
                out DataClasses.Type? type2obj
            );

            var resultPokemon = new PokemonDefinition
            {
                Name = pokemon.identifier,
                Stats = stats,
                Type1 = type1obj,
                Type2 = type2obj,
                Learnset = learnset,
            };

            return resultPokemon;
        }

        private static LearnSet GetLearnset(DatabaseContext db, models.Pokemon pokemon)
        {
            var moves = db.PokemonMoves.Where((m) => m.pokemon_id == pokemon.pokemon_id);
            int movesCount = moves.Count();
            var learnMoves = new LearnMove[movesCount];
            int i = 0;
            foreach (var move in moves)
            {
                var moveDetails = db.Moves.Where((m) => m.move_id == move.move_id).Single();
                var moveObj = new Move
                {
                    Name = moveDetails.identifier,
                    Accuracy = moveDetails.accuracy,
                    Category = (Category)moveDetails.damage_class_id,
                    Power = moveDetails.power,
                    PP = moveDetails.pp,
                    Priority = moveDetails.priority,
                    Type = (DataClasses.Type)moveDetails.type_id,
                };
                learnMoves[i] = new LearnMove
                {
                    Move = moveObj,
                    Level = move.level,
                    isNatural = move.learning_method == LEVEL_UP_MOVE_LEARN_METHOD,
                };
                i++;
            }
            return new LearnSet { Moves = learnMoves };
        }

        private static void GetPokemonTypes(
            DatabaseContext db,
            models.Pokemon pokemon,
            out DataClasses.Type type1obj,
            out DataClasses.Type? type2obj
        )
        {
            var types = db.PokemonTypes.Where((t) => t.pokemon_id == pokemon.pokemon_id);
            int? GetType(int id) => types.Where((t) => t.slot == id).SingleOrDefault()?.type_id;
            var type1 =
                GetType(1)
                ?? throw new MissingFieldException(
                    $"missing type1 in pokemon: {pokemon.identifier}"
                );
            var type2 = GetType(2);
            type1obj = (DataClasses.Type)type1;
            type2obj = type2 != null ? (DataClasses.Type)type2 : null;
        }

        private static void GetStats(
            DatabaseContext db,
            models.Pokemon pokemon,
            out Pokestats pokestats
        )
        {
            var stats = db.PokemonStats.Where((s) => s.pokemon_id == pokemon.pokemon_id);
            int GetStat(int id) => stats.Single(s => s.stat_id == id).base_stat;
            var hp = GetStat(1);
            var attack = GetStat(2);
            var defense = GetStat(3);
            var spattack = GetStat(4);
            var spdefense = GetStat(5);
            var speed = GetStat(6);
            pokestats = new Pokestats
            {
                HP = hp,
                Attack = attack,
                Defense = defense,
                SpAttack = spattack,
                SpDefense = spdefense,
                Speed = speed,
            };
        }
    }
}
