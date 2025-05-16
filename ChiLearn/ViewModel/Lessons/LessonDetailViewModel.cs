using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System.Diagnostics;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Lessons
{
    public partial class LessonDetailViewModel : BaseNotifyObject
    {

        private IRuleService _ruleService;


        private Rule _selectedRule;
        public Rule SelectedRule
        {
            get => _selectedRule;
            set => SetProperty(ref _selectedRule, value);
        }


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
        public ICommand NavagateToRuleCommand { get; }
        #endregion

        private readonly ILessonService _lessonService;

        public LessonDetailViewModel(
            ILessonService lessonService,
            IRuleService ruleService)
        {
            _lessonService = lessonService;
            _ruleService = ruleService;

            #region InitCommands
            NavigateToTheoryCommand = new Command(async () => await NavigateToTheoryPage());
            NavigateToPracticeCommand = new Command(async () => await NavigateToPracticePage());
            NavagateToRuleCommand = new Command(async () => await NavigateToRulePage());
            #endregion

        }

        public async Task Initialize(int lessonId)
        {
            try
            {
                SelectedLesson = await _lessonService.GetLessonsById(lessonId);
                SelectedRule = await _ruleService.GetRuleByLevel(lessonId);
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
            catch (Exception ex)
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

        private async Task NavigateToRulePage()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    {"RuleId", SelectedRule.Id}
                };
                await Shell.Current.GoToAsync("RuleDetailPage", parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
        }

    }
}
