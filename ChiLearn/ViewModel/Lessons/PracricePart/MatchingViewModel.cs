using ChiLearn.Abstractions;
using ChiLearn.Models;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace ChiLearn.View.LessonsView.PracticeView
{
    public class MatchingViewModel : BaseNotifyObject
    {
        private readonly ILessonService _lessonService;

        public MatchingViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;

            OnShowMistakesCommand = new Command(OnShowMistakes);
            SubmitMatchingCommand = new Command(OnSubmitMatching);
            RetryCommand = new Command(OnRetry);
            GoToTheoryCommand = new Command(OnGoToTheory);
            GoToPronunciationCommand = new Command(OnGoToPronunciation);
        }

        public ObservableCollection<Word> Words { get; } = new();
        public ObservableCollection<string> ChineseWords { get; } = new();
        public ObservableCollection<string> RussianWords { get; } = new();
        public ObservableCollection<string> Mistakes { get; } = new();

        public ObservableCollection<MatchingItem> Matches { get; } = new();
        public Dictionary<string, bool> MatchingResults { get; } = new();

        private Lesson _tLesson;
        public Lesson TLesson
        {
            get => _tLesson;
            set => SetProperty(ref _tLesson, value);
        }

        private bool _isMatchingSuccessful;
        public bool IsMatchingSuccessful
        {
            get => _isMatchingSuccessful;
            set => SetProperty(ref _isMatchingSuccessful, value);
        }

        private bool _showRetry;
        public bool ShowRetry
        {
            get => _showRetry;
            set => SetProperty(ref _showRetry, value);
        }
        private bool _showCheck;
        public bool ShowCheck
        {
            get => _showCheck;
            set => SetProperty(ref _showCheck, value);
        }

        private bool _showContinue;
        public bool ShowContinue
        {
            get => _showContinue;
            set => SetProperty(ref _showContinue, value);
        }

        private bool _showMistakes;
        public bool ShowMistakes
        {
            get => _showMistakes;
            set => SetProperty(ref _showMistakes, value);
        }

        public ICommand SubmitMatchingCommand { get; }
        public ICommand RetryCommand { get; }
        public ICommand GoToTheoryCommand { get; }
        public ICommand GoToPronunciationCommand { get; }

        internal void Initialize(Lesson lesson)
        {
            if (lesson == null) throw new ArgumentNullException(nameof(lesson));
            if (lesson.Words == null) throw new ArgumentException("Lesson words cannot be null");

            Words.Clear();
            ChineseWords.Clear();
            Matches.Clear();
            RussianWords.Clear();

            ShowRetry = false;
            ShowContinue = false;
            ShowCheck = true;

            foreach (var word in lesson.Words)
            {
                Words.Add(word);
                ChineseWords.Add(word.ChiWord);
                RussianWords.Add(word.RuWord);

                Matches.Add(new MatchingItem
                {
                    ChiWord = word.ChiWord,
                    SelectedRu = null
                });
            }

            RussianWords.Shuffle();

            TLesson = lesson;

            OnPropertyChanged(nameof(Matches));
        }

        public ICommand OnShowMistakesCommand { get; }
        private void OnShowMistakes()
        {
            ShowMistakes = !ShowMistakes;
        }

        private void OnSubmitMatching()
        {
            MatchingResults.Clear();
            Mistakes.Clear();

            bool allCorrect = true;

            foreach (var item in Matches)
            {
                var word = Words.FirstOrDefault(w => w.ChiWord == item.ChiWord);
                bool isCorrect = word != null && word.RuWord == item.SelectedRu;
                MatchingResults[item.ChiWord] = isCorrect;

                if (!isCorrect)
                {
                    allCorrect = false;
                    Mistakes.Add($"{item.ChiWord} → {item.SelectedRu ?? "ничего"} (нужно: {word?.RuWord})");
                }
            }


            IsMatchingSuccessful = allCorrect;
            ShowRetry = !allCorrect;
            ShowContinue = allCorrect;
            ShowCheck = false;

            OnPropertyChanged(nameof(MatchingResults));
            OnPropertyChanged(nameof(Mistakes));
        }

        private void OnRetry()
        {
            Mistakes.Clear();
            MatchingResults.Clear();
            IsMatchingSuccessful = false;

            ShowRetry = false;
            ShowContinue = false;
            ShowCheck = true;

            RussianWords.Shuffle();

            OnPropertyChanged(nameof(Mistakes));
            OnPropertyChanged(nameof(MatchingResults));
            OnPropertyChanged(nameof(RussianWords));
        }

        private async void OnGoToTheory()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                {"SelectedLesson", TLesson}
                };
                await Shell.Current.GoToAsync("TheoryPage", parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }


        private async void OnGoToPronunciation()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                {"SelectedLesson", TLesson}
                };
                await Shell.Current.GoToAsync("TheoryPage", parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }
    }



    public static class ShuffleExtension
    {
        private static Random rng = new();

        public static void Shuffle<T>(this ObservableCollection<T> collection)
        {
            var list = collection.ToList();
            collection.Clear();
            foreach (var item in list.OrderBy(_ => rng.Next()))
                collection.Add(item);
        }
    }


}
