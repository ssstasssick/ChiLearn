using ChiLearn.ViewModel.Lessons;

namespace ChiLearn.View;

public partial class LessonsPage : ContentPage
{
	private readonly LessonPageViewModel _viewModel;

	public LessonsPage(LessonPageViewModel viewModel)
	{

		InitializeComponent();

		BindingContext = _viewModel = viewModel;

	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_viewModel.Lessons == null || _viewModel.Lessons.Count == 0 )
		{
			await _viewModel.LoadLessonsAsync();
		}
	}

}