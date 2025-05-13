using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Services
{
    public class AudioPlayerService
    {
        private static string _audioPath;
        private IAudioPlayer? _audioPlayer;
        public AudioPlayerService(string audioPath)
        {
            _audioPath = SetAudioPath(audioPath);
        }
        public static string SetAudioPath(string audioName) 
        { 
            _audioPath = Path.Combine("Audio", audioName) + ".wav";
            return _audioPath;
        }

        public async Task PlayAudioAsync()
        {
            try
            {
                _audioPlayer?.Dispose();

                using var stream = await FileSystem.OpenAppPackageFileAsync(_audioPath);

                _audioPlayer = AudioManager.Current.CreatePlayer(stream);
                _audioPlayer.Play();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось воспроизвести аудио: {ex.Message}", "OK");
            }
        }
    

    }
}
