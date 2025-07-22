namespace Src.Misc
{
    // Idea: play a specific song depending on your opponent's type1.
    // so for like for like ghost type play lavander town.
    public enum Sounds : byte
    {
        TitleScreenSong,
        ButtonPress,
    }

    public static class SoundPaths
    {
        public static Sounds Get(string sound)
        {
            if (sound.EndsWith("titleScreen.flac"))
            {
                return Sounds.TitleScreenSong;
            }
            if (sound.EndsWith("buttonPress.flac"))
            {
                return Sounds.ButtonPress;
            }
            throw new NotSupportedException($"Could not find sound {sound}.");
        }
    }
}
