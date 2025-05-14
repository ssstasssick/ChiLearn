using ChiLearn.View.LessonsView.PracticeView;
using ChiLearn.ViewModel;

namespace ChiLearn.View
{
    [QueryProperty("LessonCompleted", "lessonCompleted")]
    public partial class MainPage : ContentPage
    {
        public bool LessonCompleted { get; set; }
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await ((MainViewModel)BindingContext).InitiazeValues();

        }

    }

}
