namespace src.Pokemon
{
    public struct Learnset
    {
        public required LearnMove[] Moves;

        public struct LearnMove
        {
            public required int Level;
            public required Move Move;
        }
    }
}
