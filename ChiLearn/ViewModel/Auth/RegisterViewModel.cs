using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Auth
{
    public class RegisterViewModel : BaseNotifyObject
    {
        private string name;
        private string email;
        private string password;

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => email;
            set { email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand CloseCommand { get; }

        private readonly ILessonService _lessonService;

        public RegisterViewModel(ILessonService lessonService)
        {
            _lessonService = lessonService;

            RegisterCommand = new Command(OnRegister);
            CloseCommand = new Command(OnClose);
        }

        private async void OnRegister()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Все поля должны быть заполнены.", "OK");
                return;
            }

            var lastLesson = await _lessonService.GetLastCompletedLessonAsync();
            var lastLevelNum = lastLesson is not null ? lastLesson.LessonNum + 1 : 1;

            var registerRequest = new RegisterRequest
            {
                Username = Name,
                Email = Email,
                Password = Password,
                LessonNum = lastLevelNum,
            };

            try
            {
                var response = await HttpClientSingleton.Instance.PostAsJsonAsync(
                                    "http://10.0.2.2:5065/api/Auth/register",
                                    registerRequest);

                if (response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех", "Вы успешно зарегистрированы!\nТеперь пройдите авторизацию.", "OK");
                    await Shell.Current.Navigation.PopModalAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Регистрация не удалась. Попробуйте снова.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        private async void OnClose()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}
