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

namespace ChiLearn.ViewModel
{
    public class MainViewModel : BaseNotifyObject
    {
        private readonly ILessonService _lessonService;
        private readonly IServiceProvider _services;
        public ICommand RegisterButtonCommand { get; }



        private int _hskLevel;
        private double _percentCompletedLevels;
        private double _progressBarPercent;
        private int _numOfLastLesson;

        public int HskLevel
        {
            get => _hskLevel;
            set
            {
                SetProperty(ref _hskLevel, value);
            }
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



        public MainViewModel(ILessonService lessonService, IServiceProvider services)
        {
            _services = services;
            _lessonService = lessonService;
            _ = InitiazeValues();
            RegisterButtonCommand = new Command(async () => await RegisterButton());
        }

        public async Task InitiazeValues()
        {
            var lastLesson =  await _lessonService.GetLastCompletedLessonAsync();
            HskLevel = lastLesson is null ? 1 : lastLesson.HskLevel ?? 1;
            NumOfLastLesson = lastLesson is null ? 0 : lastLesson.LessonNum;
            var l = await _lessonService.GetCountOfLessonsByHskLevel(HskLevel);
            PercentCompletedLevels = (double)NumOfLastLesson / (await _lessonService.GetCountOfLessonsByHskLevel(HskLevel));
            ProgressBarPercent = Double.Round(PercentCompletedLevels * 100);           

        }

        private async Task RegisterButton()
        {
            var registerPage = _services.GetRequiredService<RegisterModelPage>();
            await Shell.Current.Navigation.PushModalAsync(registerPage);

        }


    }
}
