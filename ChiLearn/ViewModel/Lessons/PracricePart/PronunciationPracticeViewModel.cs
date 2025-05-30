﻿using ChiLearn.Abstractions;
using ChiLearn.Models.User;
using ChiLearn.Services;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Domain.Services;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChiLearn.View.LessonsView.PracticeView
{
    public class PronunciationPracticeViewModel : BaseNotifyObject, IDisposable
    {
        private int NEED_TO_WIN = 3;
        private readonly MauiAudioRecorder _recorder;
        private IAudioPlayer? _audioPlayer;
        private bool _disposed;
        ILessonService _lessonService;
        private readonly SpeechFlowService _speechService;

        private int _currentIndex = 0;
        private double _progress;


        private string _status = "Зажмите для записи";
        private bool _completedPractice;

        public bool CompletedPractice
        {
            get => _completedPractice;
            set
            {
                SetProperty(ref _completedPractice, value);
                _lessonService.UpdateLesson(CurrentLesson);
            }
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlayAudioCommand { get; }
        public ICommand StartRecordingCommand { get; }
        public ICommand StopRecordingCommand { get; }

        public ICommand CompleteLessonCommand { get; }

        private Word _selectedWord;
        private int _correctAnswersCount = 0;

        public Word SelectedWord
        {
            get => _selectedWord;
            set => SetProperty(ref _selectedWord, value);
        }

        public ObservableCollection<Word> Words { get; } = new();
        public Lesson CurrentLesson { get; private set; }

        public PronunciationPracticeViewModel(
            SpeechFlowService speechService,
            IAudioManager audioManager,
            ILessonService lessonService)
        {
            _speechService = speechService;
            _lessonService = lessonService;
            _recorder = new MauiAudioRecorder(audioManager);

            PlayAudioCommand = new Command(OnPlayAudio);
            CompleteLessonCommand = new Command(async () => await OnGoToLessonPage());
            StartRecordingCommand = new Command(async () => await StartRecordingAsync());
            StopRecordingCommand = new Command(async () => await StopRecordingAsync());
        }

        public void Initialize(Lesson lesson)
        {
            CurrentLesson = lesson;
            Words.Clear();
            foreach (var word in lesson.Words)
                Words.Add(word);

            SelectedWord = Words.FirstOrDefault();
        }

        private async void OnPlayAudio()
        {
            try
            {
                _audioPlayer?.Dispose();

                var audioPath = AudioPlayerService.SetAudioPath(SelectedWord.AudioPath);
                using var stream = await FileSystem.OpenAppPackageFileAsync(audioPath);

                _audioPlayer = AudioManager.Current.CreatePlayer(stream);
                _audioPlayer.Play();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось воспроизвести аудио: {ex.Message}", "OK");
            }
        }

        private async Task UpdateProgress()
        {
            var user = await UserDataService.LoadAsync();
            user.LastLevelNum = CurrentLesson.LessonNum;
            await UserDataService.SaveAsync(user);
            
            if(user.isAuth.Equals(true)) await SendUpdateRequest(user);
        }

        private async Task SendUpdateRequest(UserDataJson user)
        {
            var updateRequest = new UpdateLessonRequest
            {
                Name = user.Name,
                LessonNum = user.LastLevelNum
            };

            try
            {
                var response = await HttpClientSingleton.Instance.PostAsJsonAsync(
                    "http://10.0.2.2:5065/api/Auth/update",
                    updateRequest);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось обновить прогресс: {content}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Сетевая ошибка: {ex.Message}", "OK");
            }
        }

        public async Task StartRecordingAsync()
        {
            if (!await CheckAndRequestPermissionsAsync())
            {
                Status = "Нет разрешения на запись";
                return;
            }

            try
            {
                Status = "Запись...";
                await _recorder.StartAsync();
            }
            catch (Exception ex)
            {
                Status = $"Ошибка при записи: {ex.Message}";
            }
        }

        string GetWavFormatDetails(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReader(fs);

            reader.BaseStream.Position = 20; // Позиция параметров формата
            var audioFormat = reader.ReadInt16(); // 1 = PCM
            var channels = reader.ReadInt16();
            var sampleRate = reader.ReadInt32();
            var byteRate = reader.ReadInt32();
            var blockAlign = reader.ReadInt16();
            var bitsPerSample = reader.ReadInt16();

            return $"Format: {audioFormat} (1=PCM), Channels: {channels}, " +
                   $"SampleRate: {sampleRate}Hz, Bits: {bitsPerSample}";
        }

        private void LogFileInfo(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                Debug.WriteLine($"Информация о файле:");
                Debug.WriteLine($"- Путь: {fileInfo.FullName}");
                Debug.WriteLine($"- Размер: {fileInfo.Length} байт");
                Debug.WriteLine($"- Создан: {fileInfo.CreationTime}");
                Debug.WriteLine($"- Последнее изменение: {fileInfo.LastWriteTime}");
                Debug.WriteLine($"- Атрибуты: {fileInfo.Attributes}");
                Debug.WriteLine(GetWavFormatDetails(filePath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при проверке файла: {ex.Message}");
            }
        }

        public async Task StopRecordingAsync()
        {
            try
            {
                Status = "Остановка...";
                var filePath = await _recorder.StopAndGetAudioPathAsync();
                LogFileInfo(filePath);

                if (!File.Exists(filePath))
                {
                    Status = "Файл не найден";
                    return;
                }

                Status = "Распознавание...";
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                var sentences = await _speechService.TranscribeAndParseAsync(filePath);

                var recognizedSentence = sentences.FirstOrDefault();
                if (recognizedSentence != null)
                {
                    string recognizedText = recognizedSentence.s;
                    Status = $"Вы сказали: {recognizedText}";

                    bool matched = SelectedWord.ChiWord.Any(c => recognizedText.Contains(c));

                    if (matched)
                    {
                        _correctAnswersCount++;
                        Words.Remove(SelectedWord);

                        if (Words.Count > 0)
                        {
                            _currentIndex = _currentIndex % Words.Count;
                            SelectedWord = Words[_currentIndex];
                            Progress = _correctAnswersCount / (double)NEED_TO_WIN;
                            Status = "Правильно! Переход к следующему слову.";
                        }
                        else
                        {
                            Status = "Все слова выполнены!";
                        }

                        if (_correctAnswersCount >= NEED_TO_WIN)
                        {
                            CompletedPractice = CurrentLesson.CompletedPractice = true;
                            await UpdateProgress();
                            Status = "Практика завершена!";
                        }
                    }
                    else
                    {
                        Status = $"Вы сказали: {recognizedText}. Повторите снова.";
                    }
                }
                else
                {
                    Status = "Не удалось распознать речь. Попробуйте снова.";
                }
            }
            catch (Exception ex)
            {
                Status = $"Ошибка: {ex.Message}";
            }
        }


        public async Task<string> CopyRecordingToAppDataDirectoryAsync(string sourceFilePath)
        {
            try
            {
                // Получаем путь к AppDataDirectory
                var appDataDirectory = FileSystem.AppDataDirectory;

                // Новый путь для сохранения в AppDataDirectory
                var destinationFilePath = Path.Combine(appDataDirectory, "temp_recording.wav");

                // Копируем файл в новое местоположение
                if (File.Exists(sourceFilePath))
                {
                    // Если файл существует, копируем его в AppDataDirectory
                    File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                    Console.WriteLine($"Файл успешно скопирован в {destinationFilePath}");
                    return destinationFilePath;
                }
                else
                {
                    Debug.WriteLine("Исходный файл не найден.");
                    return null;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Ошибка при копировании файла: " + ex.Message);
                return null;
            }
        }

        private async Task<bool> CheckAndRequestPermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Microphone>();

            return status == PermissionStatus.Granted;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _audioPlayer?.Dispose();
            _disposed = true;
        }

        private async Task OnGoToLessonPage()
        {
            try
            {
                await Shell.Current.GoToAsync("LessonPage");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }
    }

}
