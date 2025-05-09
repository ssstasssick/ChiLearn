using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Behaviors
{
    public class CarouselLoopBehavior : Behavior<CarouselView>
    {
        public static readonly BindableProperty AllWordsViewedProperty =
            BindableProperty.Create(nameof(AllWordsViewed), typeof(bool), typeof(CarouselLoopBehavior), false, propertyChanged: OnAllWordsViewedChanged);

        public bool AllWordsViewed
        {
            get => (bool)GetValue(AllWordsViewedProperty);
            set => SetValue(AllWordsViewedProperty, value);
        }

        private static void OnAllWordsViewedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CarouselLoopBehavior behavior && behavior.AssociatedObject != null)
            {
                behavior.AssociatedObject.Loop = (bool)newValue;
            }
        }

        private CarouselView _carousel;

        protected override void OnAttachedTo(CarouselView bindable)
        {
            base.OnAttachedTo(bindable);
            _carousel = bindable;
        }

        protected override void OnDetachingFrom(CarouselView bindable)
        {
            base.OnDetachingFrom(bindable);
            _carousel = null;
        }

        public CarouselView AssociatedObject => _carousel;
    }
}
