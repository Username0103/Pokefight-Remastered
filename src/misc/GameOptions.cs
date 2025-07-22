using MessagePack;

namespace Src.Misc
{
    public static partial class GameOptions
    {
        private static float _musicVolume = 0.5F;
        public static float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                OptionChanged?.Invoke(typeof(GameOptions), OptionEnum.MusicVolume);
            }
        }
        private static float _sfxVolume = 0.5F;
        public static float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;
                OptionChanged?.Invoke(typeof(GameOptions), OptionEnum.SFXVolume);
            }
        }

        public static void Load()
        {
            if (!File.Exists(Utils.OptionsPath))
            {
                return;
            }
            var options = MessagePackSerializer.Deserialize<SavedOptions>(
                File.ReadAllBytes(Utils.OptionsPath)
            );
            MusicVolume = options.MusicVolume;
            SFXVolume = options.SFXVolume;
        }

        public static void Save()
        {
            var savedOptions = new SavedOptions
            {
                MusicVolume = MusicVolume,
                SFXVolume = SFXVolume,
            };
            File.WriteAllBytes(Utils.OptionsPath, MessagePackSerializer.Serialize(savedOptions));
        }

        public enum OptionEnum
        {
            MusicVolume,
            SFXVolume,
        }

        [MessagePackObject]
        public struct SavedOptions
        {
            [Key(0)]
            public required float MusicVolume;

            [Key(1)]
            public required float SFXVolume;
        }

        public static event EventHandler<OptionEnum>? OptionChanged;
    }
}
