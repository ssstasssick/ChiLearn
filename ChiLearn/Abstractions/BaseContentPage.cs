using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiLearn.Abstractions
{
    public abstract class BaseContentPage : ContentPage
    {
        protected async void ProcessAction(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)  
            {
                //await DisplayAlert(
                //    AppResources.ErrorAlert_Title,
                //    $"{AppResources.ErrorAlert_Description}: {ex.Message}",
                //    AppResources.ErrorAlert_Ok);
            }   
        }
    }
}
