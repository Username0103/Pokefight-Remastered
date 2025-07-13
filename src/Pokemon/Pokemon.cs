namespace src.Pokemon
{
    public record struct Pokemon
    {
        public required Pokestats Stats;
        public required Type Type1;
        public required Type Type2;
        public required Learnset learnset;
        public required Move[] moves;
    }
}
