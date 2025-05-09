using ChiLearn.Abstractions;
using ChiLearn.Models;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Lessons
{
    public partial class LessonPageViewModel : BaseNotifyObject
    {
        private readonly ILessonService _lessonService;
        private List<Lesson> _lessons = new List<Lesson>();
        private bool _isLoading;
        private HskLevelItem _selectedHskLevel;

        public ObservableCollection<IGrouping<int?, Lesson>> GroupedLessons { get; } = new();

        public List<Lesson> Lessons
        {
            get => _lessons;
            set => SetProperty(ref _lessons, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public HskLevelItem SelectedHskLevel
        {
            get => _selectedHskLevel;
            set
            {
                _selectedHskLevel = value;
                OnPropertyChanged();
            }
        }

        // Команды
        public ICommand LoadLessonsCommand { get; }
        public ICommand NavigateToDetailPageCommand { get; }
        public ICommand HskLevelSelectedCommand { get; }

        public LessonPageViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;

            #region Commands
            LoadLessonsCommand = new Command(async () => await LoadLessonsAsync());
            NavigateToDetailPageCommand = new Command<Lesson>(async (lesson) => await NavigateToDetailPage(lesson));
            HskLevelSelectedCommand = new Command(OnHskLevelSelected);
            #endregion
        }

        public async Task LoadLessonsAsync()
        {
            IsLoading = true;
            try
            {
                Lessons = await _lessonService.GetAllLessons();
                var groupedLessons = Lessons
                    .OrderBy(g => g.HskLevel)
                    .ThenBy(l => l.LessonNum)  // Используем ThenBy вместо второго OrderBy
                    .GroupBy(l => l.HskLevel);

                GroupedLessons.Clear();
                foreach (var lesson in groupedLessons)
                {
                    GroupedLessons.Add(lesson);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnHskLevelSelected()
        {
            if (SelectedHskLevel != null)
            {
                await LoadLessonsAsync();
            }
        }

        private async Task NavigateToDetailPage(Lesson lesson)
        {
            var parameters = new Dictionary<string, object>
            {
                { "LessonId", lesson.LessonId }
            };
            await Shell.Current.GoToAsync("LessonDetailPage", parameters);
        }
    }
}