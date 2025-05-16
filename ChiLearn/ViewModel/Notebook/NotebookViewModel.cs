using Android.App;
using ChiLearn.Abstractions;
using ChiLearn.Models.Word;
using ChiLearn.Services;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Plugin.Maui.Audio;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Notebook;

public class NotebookViewModel : BaseNotifyObject
{
    private readonly INotebookService _notebookService;
    private readonly IWordService _wordService;
    private readonly IRuleService _ruleService;

    public ICommand MarkAsFavoritesCommand { get; }
    public ICommand PlayAudioCommand { get; }
    public ICommand SetFilterCommand { get; }
    public ICommand ShowRulesCommand { get; }
    public ICommand OpenRuleCommand { get; }

    public ObservableCollection<DirectWord> LearnedWords { get; } = new();
    public ObservableCollection<DirectWord> FilteredWords { get; } = new();
    public ObservableCollection<Rule> Rules { get; } = new();

    private string _activeFilter = "All";
    private string _currentSection = "Words"; // или "Rules"

    private List<Rule> completedRules;

    private bool _isWordsEmpty;
    private bool _IsRulesEmpty;

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            FilterWords();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    private bool _isLoading;

    public bool IsWordsEmpty
    {
        get => _isWordsEmpty;
        set => SetProperty(ref _isWordsEmpty, value);
    }

    public bool IsRulesEmpty
    {
        get => _IsRulesEmpty;
        set => SetProperty(ref _IsRulesEmpty, value);
    }


    public bool IsWordsSection => _currentSection == "Words";
    public bool IsRulesSection => _currentSection == "Rules";

    public bool IsFilterAllSelected => _activeFilter == "All" && IsWordsSection;
    public bool IsFilterFavoritesSelected => _activeFilter == "Favorites" && IsWordsSection;
    public bool IsRulesSelected => IsRulesSection;

    public NotebookViewModel(INotebookService notebookService, IWordService wordService, IRuleService ruleService)
    {
        _notebookService = notebookService;
        _wordService = wordService;
        _ruleService = ruleService;

        MarkAsFavoritesCommand = new Command<DirectWord>(async directWord =>
        {
            if (directWord is not null)
            {
                directWord.IsFavorite = !directWord.IsFavorite;
                await _wordService.ChangeWordFavoritesState(directWord.WordId);
                await ApplyFilter();

            }
        });

        OpenRuleCommand = new Command<Rule>(async rule =>
        {
            var parameters = new Dictionary<string, object>
            {
                { "RuleId", rule.Id}
            };
            await Shell.Current.GoToAsync("RuleDetailPage", parameters);
        });

        PlayAudioCommand = new Command<DirectWord>(async dw =>
        {
            if (dw is not null)
                await PlayAudio(dw);
        });

        SetFilterCommand = new Command<string>(filter =>
        {
            _currentSection = "Words";
            _activeFilter = filter;

            OnPropertyChanged(nameof(IsRulesSection));
            OnPropertyChanged(nameof(IsFilterAllSelected));
            OnPropertyChanged(nameof(IsFilterFavoritesSelected));
            OnPropertyChanged(nameof(IsRulesSelected));


            ApplyFilter();
        });

        ShowRulesCommand = new Command(async () =>
        {
            _currentSection = "Rules";
            IsRulesEmpty = IsWordsEmpty = IsRulesSelected && Rules.Count.Equals(0);
            OnPropertyChanged(nameof(IsWordsSection));
            OnPropertyChanged(nameof(IsRulesSection));
            OnPropertyChanged(nameof(IsFilterAllSelected));
            OnPropertyChanged(nameof(IsFilterFavoritesSelected));
            OnPropertyChanged(nameof(IsRulesSelected));

            await LoadRulesAsync();
        });

        _ = Initialize();
    }

    public async Task Initialize()
    {
        IsLoading = true;

        var learnedWords = await _notebookService.GetLearnedWords();
        await LoadRulesAsync();

        LearnedWords.Clear();
        foreach (var w in learnedWords)
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

        await ApplyFilter();

        IsLoading = false;
    }

    private async Task LoadRulesAsync()
    {
        IsLoading = true;

        var rules = completedRules = await _ruleService.GetLearnedRules();
        Rules.Clear();
        foreach (var rule in rules)
            Rules.Add(rule);

        IsLoading = false;
    }

    private async Task ApplyFilter()
    {
        IsLoading = true;
        FilteredWords.Clear();

        IEnumerable<DirectWord> filtered = _activeFilter switch
        {
            "Favorites" => LearnedWords.Where(w => w.IsFavorite),
            _ => LearnedWords
        };

        foreach (var word in filtered)
            FilteredWords.Add(word);



        IsLoading = false;
    }

    private void FilterWords()
    {
        if (_currentSection == "Words")
            {
            // Фильтрация слов
            if (LearnedWords == null) return;

            var filteredWords = LearnedWords.AsEnumerable();

            if (IsFilterFavoritesSelected)
            {
                filteredWords = filteredWords.Where(w => w.IsFavorite);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchQuery = SearchText.Trim().ToLower();
                filteredWords = filteredWords.Where(w =>
                    w.RuWord?.ToLower().Contains(searchQuery) == true ||
                    w.ChiWord?.ToLower().Contains(searchQuery) == true ||
                    w.Pinyin?.ToLower().Contains(searchQuery) == true);
            }

            FilteredWords.Clear();
            foreach (var word in filteredWords)
            {
                FilteredWords.Add(word);
            }
        }        
        else
        {
            // Фильтрация правил
            if (completedRules == null) return;

            var filteredRules = completedRules.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchQuery = SearchText.Trim().ToLower();
                filteredRules = filteredRules.Where(r =>
                    r.Title?.ToLower().Contains(searchQuery) == true);
            }

            Rules.Clear();
            foreach (var rule in filteredRules)
            {
                Rules.Add(rule);
            }
        }

    }

    private async Task PlayAudio(DirectWord directWord)
    {
        var audioPlayerService = new AudioPlayerService(directWord.AudioPath);
        await audioPlayerService.PlayAudioAsync();
    }
}
