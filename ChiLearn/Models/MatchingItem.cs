using ChiLearn.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Models
{
    public class MatchingItem : BaseNotifyObject
    {
        public string ChiWord { get; set; }

        private string _selectedRu;
        public string SelectedRu
        {
            get => _selectedRu;
            set => SetProperty(ref _selectedRu, value);
        }
    }
}
