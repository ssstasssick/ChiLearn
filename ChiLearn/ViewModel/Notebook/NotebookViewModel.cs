using ChiLearn.Abstractions;
using ChiLearn.Models.Word;
using ChiLearn.Services;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace ChiLearn.ViewModel.Notebook
{
    public class NotebookViewModel : BaseNotifyObject
    {
        private readonly INotebookService _notebookService;
        private readonly IWordService _wordService;

        public ICommand MarkAsFavoritesCommand { get; }
        public ICommand PlayAudioCommand { get; }
        public ICommand SetFilterCommand { get; }
        public ICommand ReloadCommand { get; }
        public ObservableCollection<DirectWord> LearnedWords { get; } = new();
        public ObservableCollection<DirectWord> FilteredWords { get; } = new();

        private string _activeFilter = "All";

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isFilterAllSelected = true;
        public bool IsFilterAllSelected
        {
            get => _isFilterAllSelected;
            set => SetProperty(ref _isFilterAllSelected, value);
        }

        private bool _isFilterFavoritesSelected;
        public bool IsFilterFavoritesSelected
        {
            get => _isFilterFavoritesSelected;
            set => SetProperty(ref _isFilterFavoritesSelected, value);
        }

        public NotebookViewModel(
            INotebookService notebookService,
            IWordService wordService)
        {
            _notebookService = notebookService;
            _wordService = wordService;

            MarkAsFavoritesCommand = new Command<DirectWord>(async directWord =>
            {
                if (directWord is not null)
                {
                    directWord.IsFavorite = !directWord.IsFavorite;
                    await _wordService.ChangeWordFavoritesState(directWord.WordId);
                    ApplyFilter(); // чтобы сразу обновилось при фильтрации "Избранное"
                }
            });

            PlayAudioCommand = new Command<DirectWord>(async dw =>
            {
                if (dw is not null) await PlayAudio(dw);
            });

            SetFilterCommand = new Command<string>(filter =>
            {
                _activeFilter = filter;
                IsFilterAllSelected = filter == "All";
                IsFilterFavoritesSelected = filter == "Favorites";
                ApplyFilter();
            });

            _ = Initialize();
        }

        private async Task Initialize()
        {
            var learnedWord = await _notebookService.GetLearnedWords();

            LearnedWords.Clear();
            foreach (var w in learnedWord)
            {
                LearnedWords.Add(new DirectWord
                {
                    WordId = w.WordId,
                    AudioPath = w.AudioPath,
                    ChiWord = w.ChiWord,
                    RuWord = w.RuWord,
                    HskLevel = w.HskLevel,
                    IsFavorite = w.IsFavorite,
                    Pinyin = w.Pinyin
                });
            }

            ApplyFilter();

        }

        private void ApplyFilter()
        {
            try
            {
                IsLoading = true;
                FilteredWords.Clear();

                var filtered = _activeFilter switch
                {
                    "Favorites" => LearnedWords.Where(w => w.IsFavorite),
                    _ => LearnedWords
                };

                foreach (var word in filtered)
                    FilteredWords.Add(word);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task PlayAudio(DirectWord directWord)
        {
            AudioPlayerService audioPlayerService = new AudioPlayerService(directWord.AudioPath);
            await audioPlayerService.PlayAudioAsync();
        }
    }

}

