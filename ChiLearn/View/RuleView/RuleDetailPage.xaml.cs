using ChiLearn.ViewModel.RuleVM;
using Core.Domain.Entity;

namespace ChiLearn.View.RuleView;

public partial class RuleDetailPage : ContentPage, IQueryAttributable
{
	public RuleDetailPage(RuleViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("RuleId", out var ruleId) &&
            ruleId is int r)
        {
            if (BindingContext is RuleViewModel viewModel)
            {
                _ = viewModel.Initialize(r);
            }
        }
    }
}