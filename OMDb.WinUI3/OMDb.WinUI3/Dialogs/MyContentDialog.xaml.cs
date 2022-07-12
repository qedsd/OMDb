using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class MyContentDialog : ContentDialog
    {
        public Button PrimaryButton { get => Button_Primary; }
        public Button CancelButton { get => Button_Cancel; }
        public TextBlock TitleTextBlock { get => TextBlock_Title; }
        public Frame ContentFrame { get => Frame_Content;}
        public ContentDialogResult DialogResult { get;private set; }
        public MyContentDialog()
        {
            XamlRoot = MainWindow.Instance.Content.XamlRoot;
            this.InitializeComponent();
            RequestedTheme = Services.ThemeSelectorService.Theme;
        }

        private void Button_Primary_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ContentDialogResult.Primary;
            this.Hide();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = ContentDialogResult.None;
            this.Hide();
        }
        public async new System.Threading.Tasks.Task<ContentDialogResult> ShowAsync()
        {
            Helpers.DialogHelper.Current = this;
            Helpers.DialogHelper.InfoBar = InfoBar;
            await base.ShowAsync();
            return DialogResult;
        }

    }
}
