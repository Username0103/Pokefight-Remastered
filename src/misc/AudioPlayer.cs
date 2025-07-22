using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;
using static Src.Misc.Sound;

namespace Src.Misc
{
    public class AudioPlayer
    {
        private readonly Dictionary<Sounds, string> sound2ResourcePath;
        private SoundPlayer? currentPlayer;
        private readonly bool isMusic;

        public AudioPlayer(bool isMusic)
        {
            _ = AudioEngineSingleton.Instance;
            var songPaths = Utils.GetResourcesWithEnding(".flac");
            sound2ResourcePath = songPaths.ToDictionary(ResourceNameToSound, v => v);
            this.isMusic = isMusic;
        }

        public void Play(Sounds sound)
        {
            Stop();
            var player = new SoundPlayer(
                new StreamDataProvider(
                    Utils.assembly.GetManifestResourceStream(sound2ResourcePath[sound])
                        ?? throw new SystemException(
                            $"Could not get song resource {sound2ResourcePath[sound]}"
                        )
                )
            );
            Mixer.Master.AddComponent(player);
            void UpdateVolume() =>
                player.Volume = isMusic ? GameOptions.MusicVolume : GameOptions.SFXVolume;
            UpdateVolume();
            GameOptions.OptionChanged += (_, o) =>
            {
                if (
                    (isMusic && (o == GameOptions.OptionEnum.MusicVolume))
                    || (!isMusic && (o == GameOptions.OptionEnum.SFXVolume))
                )
                {
                    UpdateVolume();
                }
            };
            player.IsLooping = isMusic;
            SoundToLoopPoints(sound, out var loopStart, out var loopEnd);
            if (loopStart != null)
            {
                player.SetLoopPoints((float)loopStart, loopEnd);
            }
            player.Play();
            currentPlayer = player;
        }

        public void Stop()
        {
            if (currentPlayer != null)
            {
                currentPlayer.Stop();
                Mixer.Master.RemoveComponent(currentPlayer);
                currentPlayer = null;
            }
        }

        public sealed class AudioEngineSingleton
        {
            private static readonly Lazy<AudioEngineSingleton> lazyInstance = new(() =>
            {
                _ = new MiniAudioEngine(48000, Capability.Playback);
                return new AudioEngineSingleton();
            });
            public static AudioEngineSingleton Instance
            {
                get { return lazyInstance.Value; }
            }

            private AudioEngineSingleton() { }
        }
    }
}
