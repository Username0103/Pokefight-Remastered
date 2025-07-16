using System.ComponentModel.DataAnnotations.Schema;

namespace Src.Data.models
{
    [Table("type_efficacy")]
    public class TypeEffectiveness
    {
        public int damage_type_id { get; set; }
        public int target_type_id { get; set; }

        [Column("damage_factor")]
        public int damage_factor_percent { get; set; }
    }
}
