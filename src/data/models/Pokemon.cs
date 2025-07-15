using System.ComponentModel.DataAnnotations.Schema;

namespace src.data.models
{
    [Table("pokemon_forms")]
    public class Pokemon
    {
        public int pokemon_id { get; set; }

        [Column("introduced_in_version_group_id")]
        public int generation { get; set; }
        public string identifier { get; set; } = null!;
        public bool is_default { get; set; }
    }
}
