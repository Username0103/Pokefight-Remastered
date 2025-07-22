namespace Src.Misc
{
    public static class Sound
    {
        public enum Sounds : byte
        {
            TitleScreenSong,
            BattleSong,
            ButtonPress,
        }

        public static void SoundToLoopPoints(Sounds sound, out float? start, out float? end)
        {
            start = sound switch
            {
                Sounds.BattleSong => 3,
                Sounds.TitleScreenSong => 4,
                _ => null,
            };
            end = sound switch
            {
                Sounds.TitleScreenSong => 101,
                Sounds.BattleSong => 99,
                _ => null,
            };
        }

        public static Sounds ResourceNameToSound(string sound)
        {
            if (sound.EndsWith("titleScreen.flac"))
            {
                return Sounds.TitleScreenSong;
            }
            if (sound.EndsWith("buttonPress.flac"))
            {
                return Sounds.ButtonPress;
            }
            if (sound.EndsWith("battleSong.flac"))
            {
                return Sounds.BattleSong;
            }
            throw new NotSupportedException($"Could not find sound {sound}.");
        }
    }
}
