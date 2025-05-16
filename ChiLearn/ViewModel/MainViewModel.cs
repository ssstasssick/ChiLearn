using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChiLearn.View.Auth;
using ChiLearn.Services;
using ChiLearn.Models.User;

namespace ChiLearn.ViewModel
{
    public class MainViewModel : BaseNotifyObject
    {
        private readonly ILessonService _lessonService;
        private readonly IServiceProvider _services;
        private readonly IWordService _wordService;
        public ICommand RegisterButtonCommand { get; }
        public ICommand AuthButtonCommand { get; }
        public ICommand LogOutCommand { get; }


        private int _hskLevel;
        private double _percentCompletedLevels;
        private double _progressBarPercent;
        private int _numOfLastLesson;
        private UserDataJson _userData;
        private bool _isLoggedIn;

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        public int HskLevel
        {
            get => _hskLevel;
            set
            {
                SetProperty(ref _hskLevel, value);
            }
        }        

        public UserDataJson CurrentUser
        {
            get => _userData;
            set => SetProperty(ref _userData, value);
        }


        public double ProgressBarPercent
        {
            get => _progressBarPercent;
            set => SetProperty(ref _progressBarPercent, value);
        }

        public double PercentCompletedLevels
        {
            get => _percentCompletedLevels;
            set => SetProperty(ref _percentCompletedLevels, value);
        }

        public int NumOfLastLesson
        {
            get => _numOfLastLesson;
            set => SetProperty(ref _numOfLastLesson, value);
        }        

        public MainViewModel(ILessonService lessonService, IWordService wordService, IServiceProvider services)
        {
            _services = services;
            _wordService = wordService;
            _lessonService = lessonService;
            _ = InitiazeValues();
            RegisterButtonCommand = new Command(async () => await RegisterButton());
            AuthButtonCommand = new Command(async () => await AuthorizationButton());
            LogOutCommand = new Command(async () => await LogOut());
        }

        public async Task InitiazeValues()
        {
            CurrentUser = await UserDataService.LoadAsync();
            if (CurrentUser.isAuth) await _lessonService.UpdateLastLevel(CurrentUser.LastLevelNum);

            var lastLesson =  await _lessonService.GetLastCompletedLessonAsync();
            HskLevel = lastLesson is null ? 1 : lastLesson.HskLevel ?? 1;
            NumOfLastLesson = lastLesson is null ? 0 : lastLesson.LessonNum;
            var CompletedLessonCount = NumOfLastLesson = await CalculateComletedLesson();
            var l = await _lessonService.GetCountOfLessonsByHskLevel(HskLevel);
            PercentCompletedLevels = (double)(CompletedLessonCount) / (await _lessonService.GetCountOfLessonsByHskLevel(HskLevel));
            ProgressBarPercent = Double.Round(PercentCompletedLevels * 100);
            CurrentUser = await UserDataService.LoadAsync();
            IsLoggedIn = CurrentUser is null ? false : CurrentUser.isAuth;
            

        }

        public async Task<int> CalculateComletedLesson()
        {
            if(HskLevel > 1)
            {
                return Math.Abs(await _lessonService.GetCountOfLessonsByHskLevel(HskLevel - 1) - NumOfLastLesson);
            }
            return NumOfLastLesson;
        }

        private async Task RegisterButton()
        {
            var registerPage = _services.GetRequiredService<RegisterModelPage>();
            await Shell.Current.Navigation.PushModalAsync(registerPage);

        }

        private async Task AuthorizationButton()
        {
            var registerPage = _services.GetRequiredService<AuthorizationPage>();
            await Shell.Current.Navigation.PushModalAsync(registerPage);

        }

        private async Task LogOut()
        {
            var user = new UserDataJson { Name = "Неизвестный", LastLevelNum = 1, isAuth = false };
            UserDataService.SaveAsync(user);
            await _lessonService.ResetCompletedLevels();
            await _wordService.ResetFavorites();
            await InitiazeValues();
        }


    }
}
