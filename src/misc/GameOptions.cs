using MessagePack;
using static Src.Misc.Utils;

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

        private static Speed _battleSpeed = Speed.Normal;
        public static Speed BattleSpeed
        {
            get => _battleSpeed;
            set
            {
                _battleSpeed = value;
                OptionChanged?.Invoke(typeof(GameOptions), OptionEnum.BattleSpeed);
            }
        }

        public static void Load()
        {
            if (!File.Exists(OptionsPath))
            {
                return;
            }
            var options = MessagePackSerializer.Deserialize<SavedOptions>(
                File.ReadAllBytes(OptionsPath)
            );
            MusicVolume = options.MusicVolume;
            SFXVolume = options.SFXVolume;
            BattleSpeed = options.BattleSpeed;
        }

        public static void Save()
        {
            var savedOptions = new SavedOptions
            {
                MusicVolume = MusicVolume,
                SFXVolume = SFXVolume,
                BattleSpeed = BattleSpeed,
            };
            File.WriteAllBytes(OptionsPath, MessagePackSerializer.Serialize(savedOptions));
        }

        public enum OptionEnum
        {
            MusicVolume,
            SFXVolume,
            BattleSpeed,
        }

        [MessagePackObject]
        public struct SavedOptions
        {
            [Key(0)]
            public required float MusicVolume;

            [Key(1)]
            public required float SFXVolume;

            [Key(2)]
            public required Speed BattleSpeed;
        }

        public static event EventHandler<OptionEnum>? OptionChanged;
    }
}
