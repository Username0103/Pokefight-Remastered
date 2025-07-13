namespace src.Pokemon
{
    public record struct Move
    {
        // No two-type moves in gen 1.
        public required Type Type;
        public required Category Category;
        public required int PP;
        public int Priority;
        public int Power;
        public int Accuracy;
    }
}
