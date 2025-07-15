using System.ComponentModel.DataAnnotations.Schema;

namespace src.data.models
{
    [Table("pokemon_types")]
    public class PokemonTypes
    {
        public int pokemon_id { get; set; }
        public int type_id { get; set; }
        public int slot { get; set; }
    }
}
