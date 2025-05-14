namespace ChiLearn.View.LoadingView;

public partial class LoadingPage : ContentPage
{
    private readonly string[] _messages =
    {
        "Загружаем иероглифы...",
        "Разогреваем произношение...",
        "Составляем примеры...",
        "Настраиваем грамматику...",
        "Готовим карточки..."
    };

    public LoadingPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _ = PulseLogoAsync();
        _ = CycleTextAsync(); // Запускаем смену текста
    }

    private async Task PulseLogoAsync()
    {
        while (true)
        {
            await LogoImage.ScaleTo(1.1, 800, Easing.CubicInOut);
            await LogoImage.ScaleTo(1.0, 800, Easing.CubicInOut);
        }
    }

    private async Task CycleTextAsync()
    {
        int index = 0;
        while (true)
        {
            await LoadingLabel.FadeTo(0, 500, Easing.CubicOut);
            LoadingLabel.Text = _messages[index];
            index = (index + 1) % _messages.Length;
            await LoadingLabel.FadeTo(1, 800, Easing.CubicIn);
            await Task.Delay(2000); // Пауза перед следующей сменой
        }
    }
}
