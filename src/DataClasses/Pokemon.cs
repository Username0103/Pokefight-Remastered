using static Src.Misc.Utils;

namespace Src.DataClasses
{
    public record class Pokemon
    {
        public PokemonDefinition Definition;
        public Pokestats Stats;
        public int Health;
        public MoveWithPP[] Moves;
        public int Level;

        public Pokemon(PokemonDefinition definition, Move[]? moves, int level)
        {
            Definition = definition;
            // Definitely don't put this below the other stuff or it'll be 0 by default and that causes big problems.
            Level = level;
            if (moves != null)
            {
                Moves = ExtractPP(moves);
            }
            else
            {
                Moves = ExtractPP(CalculateMoves());
            }
            Stats = new Pokestats
            {
                HP = CalculateStat(definition.Stats.HP, isHp: true),
                Speed = CalculateStat(definition.Stats.Speed, isHp: false),
                Attack = CalculateStat(definition.Stats.Attack, isHp: false),
                Defense = CalculateStat(definition.Stats.Defense, isHp: false),
                SpAttack = CalculateStat(definition.Stats.SpAttack, isHp: false),
                SpDefense = CalculateStat(definition.Stats.SpDefense, isHp: false),
            };
            Health = Stats.HP;
        }

        private static MoveWithPP[] ExtractPP(Move[] moves)
        {
            return [.. moves.Select((m) => new MoveWithPP { Move = m, PP = new PP(m.PP) })];
        }

        private int CalculateStat(int baseStat, bool isHp)
        {
            var dv = Generator.Next(0, 16); // 0-15
            // Assume some random stat EXP scaling with your level, since if you're high level you would have battled a lot of pokemon before.
            var statExp = Level * (655.35 / 5.0) * (Generator.Next(90, 101) / 100.0);
            statExp = statExp > 65535.0 ? 65535 : (int)statExp;
            var statExpTerm = Math.Floor((Math.Sqrt(Math.Max(0, statExp - 1)) + 1) / 4.0);
            var core = ((baseStat + dv) * 2.0 + statExpTerm) * Level / 100.0;
            var result = core + (isHp ? Level + 10.0 : 5.0);
            return (int)Math.Floor(result);
        }

        private Move[] CalculateMoves()
        {
            {
                var moves = Definition.Learnset.Moves;
                return
                [
                    .. (
                        from m in moves
                        where m.isNatural == true && m.Level <= Level
                        orderby m.Level
                        select m.Move
                    ).TakeLast(4),
                ];
            }
        }
    }
}
