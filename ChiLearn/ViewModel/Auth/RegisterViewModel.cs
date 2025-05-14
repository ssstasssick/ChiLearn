using ChiLearn.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public RegisterViewModel()
        {
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

            // Регистрация (здесь — заглушка)
            await Application.Current.MainPage.DisplayAlert("Успех", "Вы успешно зарегистрированы!", "OK");
            await Shell.Current.Navigation.PopModalAsync();
        }

        private async void OnClose()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }
    }
}
