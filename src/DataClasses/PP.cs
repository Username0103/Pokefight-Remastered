namespace Src.DataClasses
{
    public record class PP(int max)
    {
        public readonly int max = max;
        public int current = max;
    }
}
