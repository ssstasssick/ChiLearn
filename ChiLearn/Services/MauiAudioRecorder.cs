using Plugin.Maui.Audio;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChiLearn.Services
{
    public class MauiAudioRecorder
    {
        private readonly IAudioManager _audioManager;
        private IAudioRecorder _recorder;
        private string _tempFilePath;

        public MauiAudioRecorder(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task StartAsync()
        {
            _tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"temp_recording.wav");
            _recorder = _audioManager.CreateRecorder();
            await _recorder.StartAsync(_tempFilePath);
        }

        public async Task<string> StopAndGetAudioPathAsync()
        {
            if (_recorder != null && _recorder.IsRecording)
            {
                await _recorder.StopAsync();
            }

            return _tempFilePath;
        }

        public string GetRecordedFilePath() => _tempFilePath;
    }
}

