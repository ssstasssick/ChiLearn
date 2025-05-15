using ChiLearn.View;
using ChiLearn.View.Auth;
using ChiLearn.View.LessonsView.PracticeView;
using ChiLearn.View.LessonsView.TheoryView;
using ChiLearn.View.Notebook;
using ChiLearn.View.RuleView;


namespace ChiLearn
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute("NotebookPage", typeof(NotebookPage));
            Routing.RegisterRoute("LessonPage", typeof(LessonsPage));
            Routing.RegisterRoute("LessonDetailPage", typeof(LessonDetailPage));
            Routing.RegisterRoute("TheoryPage", typeof(TheoryPage));
            Routing.RegisterRoute("MatchingPage", typeof(MatchingPage));
            Routing.RegisterRoute("PronunciationPracticePage", typeof(PronunciationPracticePage));
            Routing.RegisterRoute("RuleDetailPage", typeof(RuleDetailPage));
            Routing.RegisterRoute("RegisterModelPage", typeof(RegisterModelPage));
            Routing.RegisterRoute("AuthorizationPage", typeof(AuthorizationPage));

        }
    }
}
