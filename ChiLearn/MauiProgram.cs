using Core.Domain.Abstractions.Sevices;
using Microsoft.Extensions.Logging;
using ChiLearn.Infrastructure;
using Core;
using ChiLearn.View;
using ChiLearn.ViewModel.Lessons;
using System.Diagnostics;
using Infrastructure.Persistence.Sqlite.Configuration;
using ChiLearn.View.LessonsView.TheoryView;
using ChiLearn.ViewModel.Lessons.TheoryPart;
using Plugin.Maui.Audio;
using ChiLearn.View.LessonsView.PracticeView;
using ChiLearn.Services;
using ChiLearn.ViewModel;
using ChiLearn.View.Notebook;
using ChiLearn.ViewModel.Notebook;
using ChiLearn.ViewModel.RuleVM;
using ChiLearn.View.RuleView;
using ChiLearn.View.Auth;
using ChiLearn.ViewModel.Auth;

namespace ChiLearn
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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


            CopyCsvFilesToAppData().ConfigureAwait(false);
            CopyJsonFilesToAppData().ConfigureAwait(false);

            return builder.Build();
        }

        private static MauiAppBuilder RegisterAppService(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddSingleton<AppShell>()
                .AddSingleton(AudioManager.Current)
                .AddSingleton(new SpeechFlowService(
                        "JRAWPAHaEh7cZdGE",
                        "kBbaTiK4pjXOrPIy"
));
            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddTransient<LessonPageViewModel>()
                .AddTransient<LessonDetailViewModel>()
                .AddTransient<TheoryViewModel>()
                .AddTransient<MatchingViewModel>()
                .AddTransient<PronunciationPracticeViewModel>()
                .AddTransient<MainViewModel>()
                .AddTransient<NotebookViewModel>()
                .AddTransient<RuleViewModel>()
                .AddTransient<RegisterViewModel>();


            return mauiAppBuilder;
        }

        private static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
        {
            _ = mauiAppBuilder.Services
                .AddTransient<LessonsPage>()
                .AddTransient<LessonDetailPage>()
                .AddTransient<TheoryPage>()
                .AddTransient<MatchingPage>()
                .AddTransient<PronunciationPracticePage>()
                .AddTransient<MainPage>()
                .AddTransient<NotebookPage>()
                .AddTransient<RuleDetailPage>()
                .AddTransient<RegisterModelPage>();


            return mauiAppBuilder;
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

        public static async Task CopyJsonFilesToAppData()
        {
            var targetDir = Path.Combine(FileSystem.AppDataDirectory, "Resources", "Raw");

            Directory.CreateDirectory(targetDir);

            var targetPath = Path.Combine(targetDir, Constants.RuleJsonFileName);
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
                using var sourceStream = await FileSystem.OpenAppPackageFileAsync(Constants.RuleJsonFileName);
                using var targetStream = File.Create(targetPath);
                await sourceStream.CopyToAsync(targetStream);
            }

        }

    }
}
