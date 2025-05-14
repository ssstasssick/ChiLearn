using ChiLearn.Abstractions;
using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.ViewModel.RuleVM
{
    public class RuleViewModel : BaseNotifyObject
    {
        private readonly IRuleService _ruleService;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        private bool _isLoading;

        private Rule _selectedRule;
        public Rule SelectedRule
        {
            get => _selectedRule;
            set => SetProperty(ref _selectedRule, value);
        }

        public RuleViewModel(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        internal async Task Initialize(int ruleId)
        {
            try
            {
                IsLoading = true;
                SelectedRule = await _ruleService.GetRuleById(ruleId);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
