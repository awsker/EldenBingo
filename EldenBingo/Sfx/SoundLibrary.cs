using EldenBingo.Util;

namespace EldenBingo.Sfx
{
    public enum SoundType
    {
        SquareClaimed,
        Bingo,
    }

    public class SoundLibrary : IDisposable
    {
        private static string SfxPath = "./Sfx/";
        private static readonly string[] AudioFiles = new string[]
        {
            "square_claimed.wav",
            "bingo.wav"
        };

        private readonly SFML.Audio.Sound?[] _sounds;

        public SoundLibrary()
        {
            _sounds = new SFML.Audio.Sound?[AudioFiles.Length];
            for (int i = 0; i < AudioFiles.Length; i++)
            {
                try
                {
                    var path = Path.Combine(SfxPath, AudioFiles[i]);
                    if (File.Exists(path))
                    {
                        var bytes = File.ReadAllBytes(path);
                        _sounds[i] = new SFML.Audio.Sound(new SFML.Audio.SoundBuffer(bytes));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    _sounds[i] = null;
                }
            }
        }

        public void PlaySound(SoundType type, int? volume = null)
        {
            int i = (int)type;
            try
            {
                if (i >= 0 && i < _sounds.Length)
                {
                    var s = _sounds[i];
                    if (s != null)
                    {
                        if (!volume.HasValue)
                            volume = Properties.Settings.Default.SoundVolume;
                        s.Volume = Math.Clamp(volume.Value, 0, 100);
                        s?.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void Dispose()
        {
            foreach (var s in _sounds)
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
        }
    }
}
