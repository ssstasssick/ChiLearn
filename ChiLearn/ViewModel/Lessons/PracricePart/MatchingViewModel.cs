using ChiLearn.Abstractions;
using ChiLearn.Models;
using ChiLearn.Models.User;
using ChiLearn.Services;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Windows.Input;

namespace ChiLearn.View.LessonsView.PracticeView
{
    public class MatchingViewModel : BaseNotifyObject
    {

        private List<Word> _wordSequence = new();
        private int _currentIndex = 0;
        private List<string> _shuffledRuWords;

        private ILessonService _lessonService;

        public MatchingViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;

            CompleteLessonCommand = new Command(async () => await OnGoToLessonPage());
            CheckMatchingCommand = new Command(OnSubmitMatching);
            WordClickCommand = new Command<string>(OnWordClick);
            SubmitMatchingCommand = new Command(OnSubmitMatching);
            RetryCommand = new Command(OnRetry);
            OnShowMistakesCommand = new Command(() => ShowMistakes = !ShowMistakes);
            GoToTheoryCommand = new Command(OnGoToTheory);
            GoToPronunciationCommand = new Command(OnGoToPronunciation);

        }

        public ObservableCollection<string> ChineseWords { get; } = new();
        public ObservableCollection<string> SelectedWords { get; } = new();
        public ObservableCollection<string> Mistakes { get; } = new();
        public Dictionary<string, bool> MatchingResults { get; } = new();

        public string? CurrentRuWord => _shuffledRuWords != null && _currentIndex < _shuffledRuWords.Count
                                            ? _shuffledRuWords[_currentIndex]
                                            : null;
        public bool ShowCheck => _currentIndex >= _wordSequence.Count;

        public ICommand WordClickCommand { get; }
        public ICommand SubmitMatchingCommand { get; }
        public ICommand RetryCommand { get; }
        public ICommand OnShowMistakesCommand { get; }
        public ICommand GoToTheoryCommand { get; }
        public ICommand GoToPronunciationCommand { get; }
        public ICommand CheckMatchingCommand { get; }
        public ICommand SelectedWordClickCommand { get; }
        public ICommand CompleteLessonCommand { get; }

        public bool _isInternetConn;
        public bool IsInternetConn
        {
            get => _isInternetConn;
            private set
            {
                SetProperty(ref _isInternetConn, value);
                _lessonService.UpdateLesson(TLesson);
            }
        }

        private Lesson _tLesson;
        public Lesson TLesson
        {
            get => _tLesson;
            set => SetProperty(ref _tLesson, value);
        }

        private bool _isMatchingSuccessful;
        public bool IsMatchingSuccessful
        {
            get => _isMatchingSuccessful && IsInternetConn;
            set => SetProperty(ref _isMatchingSuccessful, value);
        }

        private bool _showRetry;
        public bool ShowRetry
        {
            get => _showRetry;
            set => SetProperty(ref _showRetry, value);
        }

        private bool _showContinue;
        public bool ShowContinue
        {
            get => _showContinue;
            set => SetProperty(ref _showContinue, value);
        }

        private bool _showCheck;
        public bool ShowCheckButt
        {
            get => _showCheck;
            set => SetProperty(ref _showCheck, value);
        }

        private bool _showMistakes;
        public bool ShowMistakes
        {
            get => _showMistakes;
            set => SetProperty(ref _showMistakes, value);
        }

        public void Initialize(Lesson lesson)
        {
            if (lesson == null || lesson.Words == null)
                throw new ArgumentException("Lesson or its words cannot be null");

            TLesson = lesson;
            _wordSequence = lesson.Words.ToList();

            ChineseWords.Clear();
            SelectedWords.Clear();
            Mistakes.Clear();
            MatchingResults.Clear();

            // Перемешиваем китайские слова
            var shuffledChinese = _wordSequence.Select(w => w.ChiWord).ToList();
            shuffledChinese.Shuffle();

            // Перемешиваем русские слова
            var shuffledRussian = _wordSequence.Select(w => w.RuWord).ToList();
            shuffledRussian.Shuffle();

            // Добавляем перемешанные китайские слова в список
            foreach (var chi in shuffledChinese)
                ChineseWords.Add(chi);

            // Добавляем перемешанные русские слова в список SelectedWords
            foreach (var ru in shuffledRussian)
                SelectedWords.Add("");

            // Сохраняем новый порядок русских слов для использования при отображении
            _shuffledRuWords = shuffledRussian;

            _currentIndex = 0;
            ShowRetry = false;
            ShowContinue = false;
            ShowCheckButt = false;

            OnPropertyChanged(nameof(ChineseWords));
            OnPropertyChanged(nameof(SelectedWords));
            OnPropertyChanged(nameof(CurrentRuWord));
        }

        private void OnWordClick(string selectedChi)
        {
            if (_currentIndex >= _wordSequence.Count) return;

            SelectedWords[_currentIndex] = selectedChi;
            ChineseWords.Remove(selectedChi);

            _currentIndex++;

            ShowCheckButt = SelectedWords.All(w => !string.IsNullOrEmpty(w));

            OnPropertyChanged(nameof(SelectedWords));
            OnPropertyChanged(nameof(ChineseWords));
            OnPropertyChanged(nameof(CurrentRuWord));

        }

        private void OnSubmitMatching()
        {
            MatchingResults.Clear();
            Mistakes.Clear();

            bool allCorrect = true;

            var translationMap = _wordSequence
                       .GroupBy(w => w.RuWord) // Группируем слова по переводу
                       .ToDictionary(g => g.Key, g => g.Select(w => w.ChiWord).ToList());

            for (int i = 0; i < _shuffledRuWords.Count; i++) // Используем _shuffledRuWords
            {
                var correctRu = _shuffledRuWords[i]; // Получаем слово из перемешанного списка
                var correctChi = _wordSequence.First(w => w.RuWord == correctRu).ChiWord; // Ищем соответствующее китайское слово
                var selectedChi = SelectedWords[i];

                bool isCorrect = false;

                if (translationMap.ContainsKey(correctRu))
                {
                    isCorrect = translationMap[correctRu].Contains(selectedChi);
                }

                MatchingResults[correctChi] = isCorrect;

                if (!isCorrect)
                {
                    allCorrect = false;
                    Mistakes.Add($"{i + 1}) {selectedChi ?? "ничего"} (нужно: {correctChi} -> {correctRu})");
                }
            }

            if (allCorrect && (IsInternetConn = CheckInternetConnectionAsync().Result))
            {
                _ = UpdateProgress();
            }

            IsMatchingSuccessful = allCorrect;
            ShowRetry = !allCorrect;
            ShowContinue = allCorrect;
            ShowCheckButt = false;

            OnPropertyChanged(nameof(MatchingResults));
            OnPropertyChanged(nameof(Mistakes));
        }


        private void OnRetry()
        {
            var shuffled = _wordSequence.Select(w => w.ChiWord).ToList();
            shuffled.Shuffle();

            ChineseWords.Clear();
            Mistakes.Clear();
            foreach (var word in shuffled)
                ChineseWords.Add(word);

            for (int i = 0; i < SelectedWords.Count; i++)
                SelectedWords[i] = "";

            _currentIndex = 0;
            IsMatchingSuccessful = false;
            ShowRetry = false;
            ShowContinue = false;
            ShowCheckButt = true;

            OnPropertyChanged(nameof(ChineseWords));
            OnPropertyChanged(nameof(SelectedWords));
            OnPropertyChanged(nameof(CurrentRuWord));
            OnPropertyChanged(nameof(MatchingResults));
            OnPropertyChanged(nameof(Mistakes));
            OnPropertyChanged(nameof(ShowCheck));
        }

        private async Task UpdateProgress()
        {
            var user = await UserDataService.LoadAsync();
            user.LastLevelNum = TLesson.LessonNum;
            await UserDataService.SaveAsync(user);
        }

        public static async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                using var ping = new Ping();
                var pingTask = ping.SendPingAsync("8.8.4.4", 1500);

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(2);
                var httpTask = client.GetAsync("http://www.gstatic.com/generate_204");

                var completedTask = await Task.WhenAny(pingTask, httpTask);

                return completedTask == pingTask
                    ? pingTask.Result.Status == IPStatus.Success
                    : httpTask.Result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
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

        private async void OnGoToTheory()
        {
            var parameters = new Dictionary<string, object> { { "SelectedLesson", TLesson } };
            await Shell.Current.GoToAsync("TheoryPage", parameters);
        }

        private async void OnGoToPronunciation()
        {
            var parameters = new Dictionary<string, object> { { "SelectedLesson", TLesson } };
            await Shell.Current.GoToAsync("PronunciationPracticePage", parameters);
        }
    }

}
