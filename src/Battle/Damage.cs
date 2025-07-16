using Src.DataClasses;
using Src.Misc;
using static Src.Misc.Utils;

namespace Src.Battle
{
    public static class Damage
    {
        public static int Calc(
            Pokemon attacker,
            Pokemon defender,
            Move move,
            Effectiveness[] effectivenesses,
            out bool wasCrit,
            out EffectivenessLevel typeEffectiveness
        )
        {
            if (move.Power == null || move.Power <= 0)
            {
                wasCrit = false;
                typeEffectiveness = EffectivenessLevel.Normal;
                return 0;
            }

            wasCrit = IsCrit(attacker, false); // temporary false.
            var critModifier = wasCrit ? 2 : 1;

            var attack =
                move.Category == Category.Physical
                    ? attacker.Stats.Attack
                    : (double)attacker.Stats.SpAttack;

            var defense =
                move.Category == Category.Physical
                    ? defender.Stats.Defense
                    : (double)defender.Stats.SpDefense;

            var StabModifier =
                (move.Type == attacker.Definition.Type1 || move.Type == attacker.Definition.Type2)
                    ? 1.5
                    : 1.0;

            var typeEffectiveness1 = CalcTypeEffectiveness(
                move.Type,
                defender.Definition.Type1,
                effectivenesses
            );

            EffectivenessLevel typeEffectiveness2;
            if (defender.Definition.Type2 != null)
            {
                typeEffectiveness2 = CalcTypeEffectiveness(
                    move.Type,
                    (DataClasses.Type)defender.Definition.Type2,
                    effectivenesses
                );
            }
            else
            {
                typeEffectiveness2 = EffectivenessLevel.Normal;
            }
            typeEffectiveness = CombineLevel(typeEffectiveness1, typeEffectiveness2);

            var typeEffectiveness1Num = ((int)typeEffectiveness1) / 100.0;
            var typeEffectiveness2Num = ((int)typeEffectiveness2) / 100.0;

            // https://bulbapedia.bulbagarden.net/wiki/Damage#Generation_I
            var damage = 2.0 * attacker.Level * critModifier;
            damage /= 5.0;
            damage += 2.0;
            damage *= (double)move.Power;
            damage *= attack / Math.Max(defense, 1.0);
            damage /= 50.0;
            damage += 2.0;
            damage *= StabModifier;
            damage *= typeEffectiveness1Num * typeEffectiveness2Num;
            damage *= Generator.Next(217, 256) / 255.0;

            return (int)damage;
        }

        private static bool IsCrit(Pokemon pokemon, bool is_high_crit_move)
        {
            var speed = (float)pokemon.Stats.Speed;
            var modifier = is_high_crit_move ? 64.0 : 512.0;
            var percent = speed * 100.0 / modifier;
            return (Generator.NextSingle() * 100.0) <= percent;
        }

        private static EffectivenessLevel CombineLevel(
            EffectivenessLevel former,
            EffectivenessLevel latter
        )
        {
            var formerNum = (int)former;
            var latterNum = (int)latter;
            var product = formerNum * latterNum;
            if (Enum.GetValues<EffectivenessLevel>().Select((v) => (int)v).Contains(product))
            {
                return (EffectivenessLevel)product;
            }
            // Fallback
            int[] nums = [formerNum, latterNum];
            return (EffectivenessLevel)nums.OrderBy((n) => n).Last();
        }

        private static EffectivenessLevel CalcTypeEffectiveness(
            DataClasses.Type source,
            DataClasses.Type target,
            Effectiveness[] effectivenesses
        )
        {
            return effectivenesses
                .Where((e) => e.Source == source && e.Target == target)
                .Single()
                .Factor;
        }
    }
}
