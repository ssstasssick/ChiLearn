using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Lessons.TheoryPart
{
    public class TheoryViewModel : BaseNotifyObject
    {
        private readonly ILessonService _lessonService;
        private readonly IAudioManager _audioManager;

        public ObservableCollection<Word> Words { get; } = new();

        private double _savePosition = 0;
        public double SavePosition
        {
            get => _savePosition;
            set
            {
                if(value > _savePosition)
                {
                    _savePosition = value;
                }
                
            }
        }

        public ICommand FinishLessonCommand { get; }
        public ICommand PlayAudioCommand { get; }

        private Lesson _tLesson;
        public Lesson TLesson
        {
            get => _tLesson;
            set => SetProperty(ref _tLesson, value);
        }

        private int _currentPosition;

        public int CurrentPosition
        {
            get => _currentPosition;
            set
            {
                SetProperty(ref _currentPosition, value);
                UpdateProgress();
                OnPropertyChanged(nameof(AllWordsViewed));

            }
        }

        private double _progressPercentage;
        public double ProgressPercentage
        {
            get => _progressPercentage;
            set => SetProperty(ref _progressPercentage, Math.Clamp(value, 0, 1));
        }

        public bool AllWordsViewed => ProgressPercentage.Equals(1);

        public TheoryViewModel(ILessonService lessonService,
            IAudioManager audioManager)
        {
            _lessonService = lessonService;
            _audioManager = audioManager;

            FinishLessonCommand = new Command(async () => await FinishLesson());
            PlayAudioCommand = new Command<Word>(async (selectedWord) => await PlayAudio(selectedWord));
        }

        internal void Initialize(Lesson lesson)
        {
            if (lesson == null) throw new ArgumentNullException(nameof(lesson));
            if (lesson.Words == null) throw new ArgumentException("Lesson words cannot be null");

            Words.Clear();
            foreach (var word in lesson.Words)
                Words.Add(word);

            TLesson = lesson;
            CurrentPosition = 0;
            UpdateProgress();
        }

        private async Task FinishLesson()
        {
            TLesson.CompletedTheory = true;
            await _lessonService.UpdateLesson(TLesson);
            await Shell.Current.GoToAsync("..");
        }

        private void UpdateProgress()
        {
            if (Words.Count == 0)
                ProgressPercentage = 0;
            else
            {
                SavePosition = (CurrentPosition + 1) / (double)Words.Count;
                ProgressPercentage = SavePosition;
            }
                
        }

        private async Task PlayAudio(Word selectedWord)
        {
            try
            {
                var audioPath = Path.Combine("Audio", selectedWord.AudioPath) + ".mp3";

                using var stream = await FileSystem.OpenAppPackageFileAsync(audioPath);
                var player = AudioManager.Current.CreatePlayer(stream);
                player.Play();

            }
            catch (FileNotFoundException)
            {
                await Application.Current.MainPage.DisplayAlert("Error",$"Файл не найден: Audio/{selectedWord.AudioPath}", "Ok");
            }
        }
    }
}
