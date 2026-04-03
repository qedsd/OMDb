using Microsoft.Maui.Controls;

namespace OMDb.Maui.Helpers
{
    public static class NavHelper
    {
        public static readonly BindableProperty NavigateToProperty =
            BindableProperty.CreateAttached(
                "NavigateTo",
                typeof(string),
                typeof(NavHelper),
                default(string));

        public static string GetNavigateTo(Element item)
        {
            return (string)item.GetValue(NavigateToProperty);
        }

        public static void SetNavigateTo(Element item, string value)
        {
            item.SetValue(NavigateToProperty, value);
        }
    }
}
