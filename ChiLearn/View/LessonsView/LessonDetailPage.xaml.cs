using ChiLearn.Abstractions;
using ChiLearn.ViewModel.Lessons;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChiLearn.View;

public partial class LessonDetailPage : ContentPage, IQueryAttributable
{

	public LessonDetailPage(
        LessonDetailViewModel viewModel)
    {
		InitializeComponent();

        BindingContext = viewModel;
        
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("LessonId", out var lessonIdObj) &&
            lessonIdObj is int lessonId)
        {
            if(BindingContext is LessonDetailViewModel vm)
            {
                vm.Initialize(lessonId).ConfigureAwait(false);
            }
            
        }
    }

}