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
            if (moves != null)
                Moves = ExtractPP(moves);
            else
            {
                Moves = ExtractPP(CalculateMoves());
            }
            Level = level;
            Stats = new Pokestats
            {
                HP = CalculateStat(definition.Stats.HP, IsHp: true),
                Speed = CalculateStat(definition.Stats.Speed, IsHp: false),
                Attack = CalculateStat(definition.Stats.Attack, IsHp: false),
                Defense = CalculateStat(definition.Stats.Defense, IsHp: false),
                SpAttack = CalculateStat(definition.Stats.SpAttack, IsHp: false),
                SpDefense = CalculateStat(definition.Stats.SpDefense, IsHp: false),
            };
            Health = Stats.HP;
        }

        private static MoveWithPP[] ExtractPP(Move[] moves)
        {
            return [.. moves.Select((m) => new MoveWithPP { Move = m, PP = new PP(m.PP) })];
        }

        private int CalculateStat(int baseStat, bool IsHp)
        {
            var dv = Generator.Next(0, 16); // 0-15
            // Assume some random stat EXP scaling with your level, since if you're high level you would have battled a lot of pokemon before.
            var statExp = Level * 65.535 * (Generator.Next(90, 111) / 100.0);
            statExp = statExp > 65535 ? 65535 : (int)statExp;
            return (int)(
                (((baseStat + dv) * 2 + Math.Sqrt(statExp) / 4) / 100) + (IsHp ? 5 : Level + 10)
            );
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
                        orderby m.Level descending
                        select m.Move
                    ).TakeLast(4),
                ];
            }
        }
    }
}
