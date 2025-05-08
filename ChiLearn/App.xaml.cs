using Microsoft.Extensions.DependencyInjection;

namespace ChiLearn
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light;
            MainPage = serviceProvider.GetRequiredService<AppShell>();
        }
    }
}
