namespace src.Pokemon
{
    public record struct Learnset
    {
        public required LearnMove[] Moves;

        public struct LearnMove
        {
            public required int Level;
            public required Move Move;
        }
    }
}
