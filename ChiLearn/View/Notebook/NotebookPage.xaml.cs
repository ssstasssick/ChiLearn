using ChiLearn.ViewModel.Notebook;

namespace ChiLearn.View.Notebook;

public partial class NotebookPage : ContentPage
{
	public NotebookPage(NotebookViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is NotebookViewModel vm)
        {
            await vm.Initialize();
        }
    }
}