using ChiLearn.View.LoadingView;
using Core.Domain.Abstractions.Sevices;


namespace ChiLearn
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            MainPage = new LoadingPage();
            Task.Run(async () =>
            {
                var initializer = serviceProvider.GetRequiredService<IDatabaseInitializer>();
                await initializer.InitializeAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = serviceProvider.GetRequiredService<AppShell>();
                });
            });

        }
    }
}
