using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI;

namespace OMDb.WinUI3
{
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance;
        public MainWindow()
        {
            this.InitializeComponent();
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
        public async void Pick()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.FileTypeFilter.Add("*");
            await openPicker.PickSingleFileAsync();
        }
    }
}
