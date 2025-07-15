using System.ComponentModel.DataAnnotations.Schema;

namespace src.data.models
{
    [Table("type_efficacy")]
    public class TypeEffectiveness
    {
        public int damage_type_id;
        public int target_type_id;

        [Column("damage_factor")]
        public int damage_factor_percent;
    }
}
