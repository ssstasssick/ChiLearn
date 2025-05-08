using ChiLearn.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Lessons
{
    
    public partial class LessonPageViewModel : BaseNotifyObject
    {

        private readonly ILessonService _lessonService;
        private List<Lesson> _lessons = new List<Lesson>();
        private bool _isLoading;

        public LessonPageViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;
            LoadLessonsCommand = new Command(async () => await LoadLessonsAsync());
        }

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

        public ICommand LoadLessonsCommand { get; }

        public async Task LoadLessonsAsync()
        {
            try
            {
                IsLoading = true;
                Lessons = await _lessonService.GetAllLessons();
            }
            finally
            {
                IsLoading = false;
            }
        }


    }
}
