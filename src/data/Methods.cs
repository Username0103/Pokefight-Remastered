using MessagePack;
using src.DataClasses;
using static src.DataClasses.LearnSet;

namespace src.data
{
    public static class Methods
    {
        private static readonly int LEVEL_UP_MOVE_LEARN_METHOD = 1;
        public static readonly string CACHE_PATH = Path.Join(
            Path.GetDirectoryName(Database.GetDbPath()),
            "DB_cache.bin"
        );

        private struct Cache
        {
            public required Pokemon[] pokemon;
            public required Effectiveness[] effectivenesses;
        }

        public static Pokemon[] GetAllPokemon(out Effectiveness[] effectivenesses)
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
                effectivenesses = cached.effectivenesses;
                return cached.pokemon;
            }
            using var db = new DatabaseContext();
            Pokemon[] result =
            [
                .. db
                    .Pokemon.Where(p => p.generation == 1 && p.is_default)
                    .Select(pokemon => BuildPokemon(db, pokemon)),
            ];
            effectivenesses = GetTypeChart(db);
            var newCache = new Cache { pokemon = result, effectivenesses = effectivenesses };
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
                    Factor = e.damage_factor_percent / 100.0,
                    Source = (DataClasses.Type)e.damage_type_id,
                    Target = (DataClasses.Type)e.target_type_id,
                },
            ];
        }

        private static Pokemon BuildPokemon(DatabaseContext db, models.Pokemon pokemon)
        {
            GetStats(db, pokemon, out Pokestats stats);
            GetMovesAndLearnset(db, pokemon, out Move[] moveObjs, out LearnSet learnset);
            GetPokemonTypes(
                db,
                pokemon,
                out DataClasses.Type type1obj,
                out DataClasses.Type? type2obj
            );

            var resultPokemon = new Pokemon
            {
                Name = pokemon.identifier,
                Stats = stats,
                EffectiveStats = stats,
                Type1 = type1obj,
                Type2 = type2obj,
                learnset = learnset,
                moves = moveObjs,
            };

            return resultPokemon;
        }

        private static void GetMovesAndLearnset(
            DatabaseContext db,
            models.Pokemon pokemon,
            out Move[] moveObjs,
            out LearnSet learnset
        )
        {
            var moves = db.PokemonMoves.Where((m) => m.pokemon_id == pokemon.pokemon_id);
            int movesCount = moves.Count();
            var learnMoves = new LearnMove[movesCount];
            moveObjs = new Move[movesCount];
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
                moveObjs[i] = moveObj;
                i++;
            }
            learnset = new LearnSet { Moves = learnMoves };
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
