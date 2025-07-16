namespace Src.Misc
{
    public enum Song : byte
    {
        TitleScreen,
    }

    public static class SongPaths
    {
        public static Song Get(string song)
        {
            if (song.EndsWith("titleScreen.flac"))
            {
                return Song.TitleScreen;
            }
            throw new NotSupportedException($"Could not find song {song}.");
        }
    }
}
