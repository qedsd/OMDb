using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using OMDb.Core.Services;
using OMDb.WinUI3.Services;
using System.Runtime.InteropServices; // For DllImport
using WinRT; // required to support Window.As<ICompositionSupportsSystemBackdrop>()


namespace OMDb.WinUI3
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "OMDb";
            Helpers.WindowHelper.TrackWindow(this);
            Helpers.WindowHelper.SetMainWindow(this);
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            RatingService.Init();
            Instance = this;
            if (Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Services.ThemeSelectorService.Theme;
            }
            if(!TrySetMicaBackdrop())
            {
                //不启用需要自行修改主背景色
                ThemeSelectorService.OnChangedTheme += ThemeSelectorService_OnChangedTheme;
                ThemeSelectorService_OnChangedTheme(ThemeSelectorService.Theme);
            }
        }

        private void ThemeSelectorService_OnChangedTheme(ElementTheme theme)
        {
            switch (theme)
            {
                case ElementTheme.Dark: MainWindowGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,32, 32, 32)); break;
                case ElementTheme.Light: MainWindowGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,243, 243, 243)); break;
                case ElementTheme.Default:
                    {
                        if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                        {
                            MainWindowGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 32, 32, 32));
                        }
                        else
                        {
                            MainWindowGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 243, 243, 243));
                        }
                    }
                    break;
            }
        }

        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Core.Config.AddConnectionString($"DataSource={System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OMDb.db")}", "1");
        //    myButton.Content = "Clicked";
        //    var s = RatingService.GetRatings("test");
        //    if (s != null)
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        foreach (var p in s)
        //        {
        //            stringBuilder.AppendLine(p.ToString());
        //        }
        //        RateTextBlock.Text = stringBuilder.ToString();
        //    }
        //}

        Helpers.WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;

        bool TrySetMicaBackdrop()
        {
            if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
            {
                m_wsdqHelper = new Helpers.WindowsSystemDispatcherQueueHelper();
                m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

                // Hooking up the policy object
                m_configurationSource = new Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration();
                this.Activated += Window_Activated;
                this.Closed += Window_Closed;
                ((FrameworkElement)this.Content).ActualThemeChanged += Window_ThemeChanged;

                // Initial configuration state.
                m_configurationSource.IsInputActive = true;
                SetConfigurationSourceTheme();

                m_micaController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();

                // Enable the system backdrop.
                // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
                m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
                return true; // succeeded
            }

            return false; // Mica is not supported on this system
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            m_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
            // use this closed window.
            if (m_micaController != null)
            {
                m_micaController.Dispose();
                m_micaController = null;
            }
            this.Activated -= Window_Activated;
            m_configurationSource = null;
        }

        private void Window_ThemeChanged(FrameworkElement sender, object args)
        {
            if (m_configurationSource != null)
            {
                SetConfigurationSourceTheme();
            }
        }

        private void SetConfigurationSourceTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Dark; break;
                case ElementTheme.Light: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Light; break;
                case ElementTheme.Default: m_configurationSource.Theme = Microsoft.UI.Composition.SystemBackdrops.SystemBackdropTheme.Default; break;
            }
        }
    }
}
