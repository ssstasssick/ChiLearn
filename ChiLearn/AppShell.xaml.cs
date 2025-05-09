using ChiLearn.View;
using ChiLearn.View.LessonsView.TheoryView;

namespace ChiLearn
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("LessonDetailPage", typeof(LessonDetailPage));
            Routing.RegisterRoute("TheoryPage", typeof(TheoryPage));
        }
    }
}
