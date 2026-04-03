using Microsoft.Maui.Controls;
using System;

namespace OMDb.Maui.Services
{
    public static class NavigationService
    {
        public static async Task NavigateAsync(Type pageType, object parameter = null)
        {
            var pageName = pageType.Name.Replace("Page", "");
            var route = $"//{pageName}Page";
            await Shell.Current.GoToAsync(route);
        }

        public static async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
