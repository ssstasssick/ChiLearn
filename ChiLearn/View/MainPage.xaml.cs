using ChiLearn.ViewModel;

namespace ChiLearn.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

    }

}
