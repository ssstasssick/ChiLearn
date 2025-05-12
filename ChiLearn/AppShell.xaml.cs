using ChiLearn.View;
using ChiLearn.View.LessonsView;
using ChiLearn.View.LessonsView.PracticeView;
using ChiLearn.View.LessonsView.TheoryView;


namespace ChiLearn
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute("LessonPage", typeof(LessonsPage));
            Routing.RegisterRoute("LessonDetailPage", typeof(LessonDetailPage));
            Routing.RegisterRoute("TheoryPage", typeof(TheoryPage));
            Routing.RegisterRoute("MatchingPage", typeof(MatchingPage));
            Routing.RegisterRoute("PronunciationPracticePage", typeof(PronunciationPracticePage));
        }
    }
}
