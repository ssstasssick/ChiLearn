
using Core.Domain.Entity;

namespace ChiLearn.View.LessonsView.PracticeView;

public partial class PronunciationPracticePage : ContentPage, IQueryAttributable
{
    private readonly PronunciationPracticeViewModel _viewModel;
    public PronunciationPracticePage(PronunciationPracticeViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = _viewModel = viewModel;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedLesson", out var lessonObj) &&
            lessonObj is Lesson l)
        {
            if (BindingContext is PronunciationPracticeViewModel viewModel)
            {
                viewModel.Initialize(l);
            }
        }
    }

    private async void OnRecordPressed(object sender, EventArgs e)
    {
        await _viewModel.StartRecordingAsync();
    }

    private async void OnRecordReleased(object sender, EventArgs e)
    {
        await _viewModel.StopRecordingAsync();
    }
}