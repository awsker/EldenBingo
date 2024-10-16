using EldenBingo.Util;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;

namespace EldenBingo.Sfx
{
    public enum SoundType
    {
        SquareClaimedOther,
        SquareClaimedOwn,
        Bingo,
    }

    public class AudioDevice
    {
        public string Name { get; private set; }
        public string Id { get; private set; }

        public AudioDevice(string Name, string Id)
        {
            this.Name = Name;
            this.Id = Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SoundLibrary : IDisposable, IMMNotificationClient
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;

        private static string SfxPath = "./Sfx/";
        private static readonly string[] AudioFiles = new string[]
        {
            "square_claimed_other.wav",
            "square_claimed_own.wav",
            "bingo.wav"
        };

        private readonly CachedSound?[] _sounds;
        private readonly WasapiOut?[] _players;

        private MMDevice? _currentDevice = null;
        private string _forceDeviceId = string.Empty;

        public SoundLibrary()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _deviceEnumerator.RegisterEndpointNotificationCallback(this);
            _sounds = new CachedSound[AudioFiles.Length];
            _players = new WasapiOut[AudioFiles.Length];
            for (int i = 0; i < AudioFiles.Length; i++)
            {
                try
                {
                    var path = Path.Combine(SfxPath, AudioFiles[i]);
                    if (File.Exists(path))
                    {
                        _sounds[i] = new CachedSound(path);
                    }
                    
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    _sounds[i] = null;
                }
            }
            initAudioDevice();
        }

        public IList<AudioDevice> GetAudioDevices()
        {
            var list = new List<AudioDevice>();
            list.Add(new AudioDevice("System Default Output", string.Empty));
            foreach(var device in _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                list.Add(new AudioDevice(device.FriendlyName, device.ID));
            }
            return list;
        }

        public void SetAudioDevice(string id)
        {
            _forceDeviceId = id;
            _currentDevice = null;
        }

        private MMDevice defaultAudioDevice()
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        }

        public void PlaySound(SoundType type, int? volume = null)
        {
            if (_currentDevice == null)
            {
                initAudioDevice();
            }
            int i = (int)type;
            try
            {
                if (i >= 0 && i < _sounds.Length)
                {
                    var s = _sounds[i];
                    if (s != null)
                    {
                        WasapiOut? p = _players[i];
                        p?.Stop();
                        p?.Dispose();
                        p = new WasapiOut(_currentDevice, AudioClientShareMode.Shared, false, 50);
                        p.Volume = Math.Clamp((volume ?? Properties.Settings.Default.SoundVolume) * 0.01f, 0f, 1f);
                        p.Init(new CachedSoundSampleProvider(s));
                        p.Play();
                        _players[i] = p;
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
            foreach(var p in _players)
            {
                p?.Dispose();
            }
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            //throw new NotImplementedException();
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            //throw new NotImplementedException();
        }

        public void OnDeviceRemoved(string deviceId)
        {
            //throw new NotImplementedException();
        }

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
            {
                MainForm.Instance?.PrintToConsole($"Default audio output device changed to: {defaultDeviceId}", Color.BlanchedAlmond, true);
                _currentDevice = null;
            }
        }

        private void initAudioDevice()
        {
            if(_currentDevice ==  null && _forceDeviceId != string.Empty)
            {
                _currentDevice = _deviceEnumerator.GetDevice(_forceDeviceId);
            }
            //If still null then the device wasn't found so use the default device instead
            if (_currentDevice == null)
            {
                _currentDevice = defaultAudioDevice();
            }
            for (int i = 0; i < _players.Length; ++i)
            {
                _players[i]?.Stop();
                _players[i]?.Dispose();
                _players[i] = null;
            }
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            //throw new NotImplementedException();
        }

        private class CachedSound
        {
            public byte[] AudioData { get; private set; }
            public WaveFormat WaveFormat { get; private set; }
            public CachedSound(string audioFileName)
            {
                using (var audioFileReader = new AudioFileReader(audioFileName))
                {
                    // TODO: could add resampling in here if required
                    WaveFormat = audioFileReader.WaveFormat;
                    var wholeFile = new List<byte>((int)(audioFileReader.Length));
                    var readBuffer = new byte[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels * 4];
                    int bytesRead;
                    while ((bytesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        wholeFile.AddRange(readBuffer.Take(bytesRead));
                    }
                    AudioData = wholeFile.ToArray();
                }
            }
        }

        private class CachedSoundSampleProvider : IWaveProvider
        {
            private readonly CachedSound cachedSound;
            private long position;

            public CachedSoundSampleProvider(CachedSound cachedSound)
            {
                this.cachedSound = cachedSound;
            }

            public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }

            public int Read(byte[] buffer, int offset, int count)
            {
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;
                return (int)samplesToCopy;
            }
        }
    }
}
