using ChiLearn.ViewModel.Lessons.TheoryPart;
using Core.Domain.Entity;

namespace ChiLearn.View.LessonsView.TheoryView;

public partial class TheoryPage : ContentPage, IQueryAttributable
{
	public TheoryPage(TheoryViewModel theoryViewModel)
	{
		InitializeComponent();

		BindingContext = theoryViewModel;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if(query.TryGetValue("SelectedLesson", out var lessonObj) &&
			lessonObj is Lesson l)
		{
			if (BindingContext is TheoryViewModel viewModel)
			{
				viewModel.Initialize(l);
			}
		}
    }
}