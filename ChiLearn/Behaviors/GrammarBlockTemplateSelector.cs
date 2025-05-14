using Core.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Behaviors
{
    public class GrammarBlockTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate BoldTemplate { get; set; }
        public DataTemplate ExampleTemplate { get; set; }
        public DataTemplate TableTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var block = item as GrammarBlock;
            return block?.Type switch
            {
                "header" => HeaderTemplate,
                "text" => TextTemplate,
                "bold" => BoldTemplate,
                "example" => ExampleTemplate,
                "table" => TableTemplate,
                _ => TextTemplate
            };
        }
    }
}
