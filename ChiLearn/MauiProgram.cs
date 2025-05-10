using Core.Domain.Abstractions.Sevices;
using Microsoft.Extensions.Logging;
using ChiLearn.Infrastructure;
using Core;
using ChiLearn.View;
using ChiLearn.ViewModel.Lessons;
using System.Diagnostics;
using Infrastructure.Persistence.Sqlite.Configuration;
using ChiLearn.View.LessonsView.TheoryView;
using CommunityToolkit.Maui;
using ChiLearn.ViewModel.Lessons.TheoryPart;
using Plugin.Maui.Audio;
using Microsoft.Maui.Storage;
using ChiLearn.View.LessonsView.PracticeView;

namespace ChiLearn
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterAppService()
                .RegisterViewModels()
                .RegisterViews();




#if DEBUG
            builder.Logging.AddDebug();
#endif

            var infrastuctureConfig = new InfrastuctureConfiguration
            {
                AppDirectoryPath = FileSystem.AppDataDirectory,
            };


            builder.Services
                .RegisterInfrastuctureService(infrastuctureConfig)
                .RegistryCoreServices();

            InitializeDatabase(builder.Build().Services).ConfigureAwait(false);

            CopyCsvFilesToAppData().ConfigureAwait(false);

            return builder.Build();
        }

        private static MauiAppBuilder RegisterAppService(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddSingleton<AppShell>()
                .AddSingleton(AudioManager.Current);
            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddTransient<LessonPageViewModel>()
                .AddTransient<LessonDetailViewModel>()
                .AddTransient<TheoryViewModel>()
                .AddTransient<MatchingViewModel>();

            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddTransient<LessonsPage>()
                .AddTransient<LessonDetailPage>()
                .AddTransient<TheoryPage>()
                .AddTransient<MatchingPage>();


            return mauiAppBuilder;
        }

        private static async Task InitializeDatabase(IServiceProvider services)
        {
            try
            {
                using var scope = services.CreateScope();
                var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
                await initializer.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization failed: {ex}");
#if DEBUG
                await Application.Current.MainPage.DisplayAlert("Error", $"DB init failed: {ex.Message}", "OK");
#endif
            }
        }

        public static async Task CopyCsvFilesToAppData()
        {
            var targetDir = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Raw");
            Directory.CreateDirectory(targetDir);

            foreach (var (hskLevel, fileName) in Constants.HskCsvFileName)
            {
                var targetPath = Path.Combine(targetDir, fileName);
                if (!File.Exists(targetPath))
                {
                    using var sourceStream = await FileSystem.OpenAppPackageFileAsync(fileName);
                    using var targetStream = File.Create(targetPath);
                    sourceStream.CopyTo(targetStream);
                }
            }
        }

        



    }
}
