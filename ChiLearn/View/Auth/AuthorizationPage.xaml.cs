using ChiLearn.ViewModel.Auth;

namespace ChiLearn.View.Auth;

public partial class AuthorizationPage : ContentPage
{
	public AuthorizationPage(AuthorizationViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}