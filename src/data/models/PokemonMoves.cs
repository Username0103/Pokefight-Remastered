using System.ComponentModel.DataAnnotations.Schema;

namespace src.data.models
{
    [Table("pokemon_moves")]
    public class PokemonMoves
    {
        public int pokemon_id { get; set; }
        public int move_id { get; set; }
        public int level { get; set; }

        [Column("pokemon_move_method_id")]
        public int learning_method { get; set; }
    }
}
