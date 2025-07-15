using System.ComponentModel.DataAnnotations.Schema;

namespace src.data.models
{
    [Table("pokemon_stats")]
    public class PokemonStats
    {
        public int pokemon_id { get; set; }
        public int stat_id { get; set; }
        public int base_stat { get; set; }
    }
}
