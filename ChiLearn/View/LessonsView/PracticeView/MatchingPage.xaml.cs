
using Core.Domain.Entity;

namespace ChiLearn.View.LessonsView.PracticeView;

public partial class MatchingPage : ContentPage, IQueryAttributable
{
	public MatchingPage(MatchingViewModel matchingViewModel)
	{
		InitializeComponent();

        BindingContext = matchingViewModel;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedLesson", out var lessonObj) &&
            lessonObj is Lesson l)
        {
            if (BindingContext is MatchingViewModel viewModel)
            {
                viewModel.Initialize(l);
            }
        }
    }
}