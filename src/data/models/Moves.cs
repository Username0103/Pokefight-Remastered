using System.ComponentModel.DataAnnotations.Schema;

namespace Src.Data.models
{
    [Table("moves")]
    public class Moves
    {
        [Column("id")]
        public int move_id { get; set; }
        public int type_id { get; set; }
        public string identifier { get; set; } = null!;
        public int pp { get; set; }
        public int damage_class_id { get; set; }
        public int priority { get; set; }
        public int? accuracy { get; set; }
        public int? power { get; set; }
    }
}
