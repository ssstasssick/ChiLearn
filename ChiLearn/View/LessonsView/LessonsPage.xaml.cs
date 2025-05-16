using ChiLearn.ViewModel.Lessons;

namespace ChiLearn.View;

public partial class LessonsPage : ContentPage
{
	private readonly LessonPageViewModel _viewModel;

	public LessonsPage(LessonPageViewModel viewModel)
	{

		InitializeComponent();

		BindingContext = _viewModel = viewModel;

        viewModel.ScrollToLessonAction = (lesson) =>
        {
            LessonsCollectionView.ScrollTo(lesson, position: ScrollToPosition.Start, animate: false);
        };

    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadLessonsAsync();
	}


}