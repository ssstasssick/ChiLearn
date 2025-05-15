using ChiLearn.Models;
using ChiLearn.Models.User;
using ChiLearn.Services;
using Core.Domain.Services;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChiLearn.ViewModel.Auth
{
    public class AuthorizationViewModel
    {
        private string _username;
        private string _password;
        private bool _isBusy;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand CloseCommand { get; }

        public AuthorizationViewModel()
        {
            LoginCommand = new Command(async () => await LoginAsync());
            CloseCommand = new Command(async () => await Shell.Current.Navigation.PopModalAsync());
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var loginRequest = new
                {
                    username = Username,
                    password = Password
                };

                var response = await HttpClientSingleton.Instance.PostAsJsonAsync(
                    "http://10.0.2.2:5065/api/Auth/login",
                    loginRequest
                );

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var userData = JsonConvert.DeserializeObject<UserDto>(content);

                    // Сохраняем данные через Preferences
                    Preferences.Set("Username", userData.Username);
                    Preferences.Set("Email", userData.Email);
                    Preferences.Set("LastLessonNum", userData.LastLessonNum);

                    var user = new UserDataJson
                    {
                        Name = Username,
                        Email = userData.Email,
                        LastLevelNum = userData.LastLessonNum,
                        isAuth = true
                    };

                    await UserDataService.SaveAsync(user);

                    await Shell.Current.DisplayAlert("Успех", "Авторизация прошла успешно", "OK");
                    await Shell.Current.Navigation.PopModalAsync();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Неверный логин или пароль", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Ошибка подключения: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
