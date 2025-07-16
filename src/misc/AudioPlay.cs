using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace Src.Misc
{
    public class AudioPlayer
    {
        private readonly Dictionary<Song, string> song2ResourcePath;
        private SoundPlayer? currentPlayer;

        public AudioPlayer()
        {
            _ = new MiniAudioEngine(48000, Capability.Playback);
            var songPaths = Utils.GetResourcesWithEnding(".flac");
            song2ResourcePath = songPaths.ToDictionary(SongPaths.Get);
        }

        public void Play(Song song)
        {
            if (currentPlayer != null)
            {
                currentPlayer.Stop();
                Mixer.Master.RemoveComponent(currentPlayer);
                currentPlayer = null;
            }
            var player = new SoundPlayer(
                new StreamDataProvider(
                    Utils.assembly.GetManifestResourceStream(song2ResourcePath[song])
                        ?? throw new SystemException(
                            $"Could not get song resource {song2ResourcePath[song]}"
                        )
                )
            );
            Mixer.Master.AddComponent(player);
            player.Volume = 0.5f;
            player.IsLooping = true;
            player.Play();
            currentPlayer = player;
        }
    }
}
