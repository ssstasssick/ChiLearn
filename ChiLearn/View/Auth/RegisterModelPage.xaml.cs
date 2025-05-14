using ChiLearn.ViewModel.Auth;

namespace ChiLearn.View.Auth;

public partial class RegisterModelPage : ContentPage
{
	public RegisterModelPage(RegisterViewModel wm)
	{

		InitializeComponent();
		BindingContext = wm;
	}
}