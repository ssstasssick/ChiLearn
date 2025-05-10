using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Lessons
{
    public partial class LessonDetailViewModel : BaseNotifyObject
    {
        private Lesson _selectedLesson;
        public Lesson SelectedLesson
        {
            get => _selectedLesson;
            set => SetProperty(ref _selectedLesson, value);
        }

        #region Commands
        public ICommand GoBackCommand { get; }
        public ICommand NavigateToTheoryCommand { get; }
        public ICommand NavigateToPracticeCommand { get; }
        #endregion

        private readonly ILessonService _lessonService;

        public LessonDetailViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;

            #region InitCommands
            NavigateToTheoryCommand = new Command(async () => await NavigateToTheoryPage());
            NavigateToPracticeCommand = new Command(async () => await NavigateToPracticePage());
            #endregion

        }

        public async Task Initialize(int lessonId)
        {
            try
            {
                SelectedLesson = await _lessonService.GetLessonsById(lessonId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки урока: {ex.Message}");
            }
        }

        private async Task NavigateToTheoryPage()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                {"SelectedLesson", SelectedLesson}
                };
                await Shell.Current.GoToAsync("TheoryPage", parameters);
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private async Task NavigateToPracticePage()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                {"SelectedLesson", SelectedLesson}
                };
                await Shell.Current.GoToAsync("MatchingPage", parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

    }
}
