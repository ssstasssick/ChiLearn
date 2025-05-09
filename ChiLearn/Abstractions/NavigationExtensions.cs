using System.Web;

namespace ChiLearn.Abstractions
{
    public static class NavigationExtensions
    {
        public static bool TryGetNavigationParameter<T>(this NavigatedToEventArgs args, string key, out T value)
        {
            if (Shell.Current?.CurrentState?.Location is not null)
            {
                var query = HttpUtility.ParseQueryString(Shell.Current.CurrentState.Location.Query);
                if (query[key] is string strValue)
                {
                    value = (T)Convert.ChangeType(strValue, typeof(T));
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
