using MessagePack;
using Src.Misc;

namespace Src.DataClasses
{
    [MessagePackObject]
    public record class Effectiveness
    {
        [Key(0)]
        public required Type Source;

        [Key(1)]
        public required Type Target;

        [Key(2)]
        public required EffectivenessLevel Factor;
    }
}
