using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.ViewModels
{
    public class SettingViewModel : ObservableObject
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }


        private int selectedThemeIndex = (int)ThemeSelectorService.Theme;
        public int SelectedThemeIndex
        {
            get => selectedThemeIndex;
            set
            {
                selectedThemeIndex = value;
                ElementTheme = (ElementTheme)value;
                _ = ThemeSelectorService.SetThemeAsync(ElementTheme);
            }
        }
    }
}
