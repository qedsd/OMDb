using Microsoft.UI;
using Microsoft.UI.Xaml;
using OMDb.WinUI3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace OMDb.WinUI3.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; } = ElementTheme.Default;

        public static bool IsDark
        {
            get
            {
                if (Theme == ElementTheme.Default)
                {
                    return Application.Current.RequestedTheme == ApplicationTheme.Dark;
                }
                else
                {
                    return Theme == ElementTheme.Dark;
                }
            }
        }

        public static void Initialize()
        {
            Theme = LoadThemeFromSettings();
            SetRequestedTheme();
            OnChangedThemeHandler?.Invoke(Theme);
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;
            SetRequestedTheme();
            OnChangedThemeHandler?.Invoke(Theme);
            await SaveThemeInSettingsAsync(Theme);
        }

        public static void SetRequestedTheme()
        {
            var res = Microsoft.UI.Xaml.Application.Current.Resources;
            Action<Windows.UI.Color> SetTitleBarButtonForegroundColor = (Windows.UI.Color color) => { res["WindowCaptionForeground"] = color; };
            switch (Theme)
            {
                case ElementTheme.Dark: SetTitleBarButtonForegroundColor(Colors.White); break;
                case ElementTheme.Light: SetTitleBarButtonForegroundColor(Colors.Black); break;
                case ElementTheme.Default:
                    {
                        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                        {
                            SetTitleBarButtonForegroundColor(Colors.White);
                        }
                        else
                        {
                            SetTitleBarButtonForegroundColor(Colors.Black);
                        }
                    }
                    break;
            }
            foreach (Window window in Helpers.WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = Theme;
                    TitleBarHelper.triggerTitleBarRepaint(WindowHelper.GetWindowForElement(Helpers.WindowHelper.MainWindow.Content));
                }
            }
        }
        private static ElementTheme LoadThemeFromSettings()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = SettingService.GetValue(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private static async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await SettingService.SetValueAsync(SettingsKey, theme.ToString());
        }
        private static ChangedThemeDelegate OnChangedThemeHandler;
        public delegate void ChangedThemeDelegate(ElementTheme theme);

        public static event ChangedThemeDelegate OnChangedTheme
        {
            add => OnChangedThemeHandler += value;
            remove => OnChangedThemeHandler-=value;
        }
    }
}
