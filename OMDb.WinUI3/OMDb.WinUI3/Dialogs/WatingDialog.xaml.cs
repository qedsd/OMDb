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
    public sealed partial class WatingDialog : Page
    {
        private static MyContentDialog Current;
        public WatingDialog(string tip = null)
        {
            this.InitializeComponent();
            if(!string.IsNullOrEmpty(tip))
            {
                TipTextBlock.Text = tip;
                TipTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                TipTextBlock.Text = string.Empty;
                TipTextBlock.Visibility = Visibility.Collapsed;
            }
        }
        public static void Show(string tip = null)
        {
            if(Current == null)
            {
                MyContentDialog dialog = new MyContentDialog();
                dialog.TitleTextBlock.Visibility = Visibility.Collapsed;
                dialog.PrimaryButton.Visibility = Visibility.Collapsed;
                dialog.CancelButton.Visibility = Visibility.Collapsed;
                WatingDialog content = new WatingDialog(tip);
                Current = dialog;
                dialog.ContentFrame.Content = content;
                _ = dialog.ShowAsync();
            }
            else
            {
                (Current.ContentFrame.Content as WatingDialog).TipTextBlock.Text = tip;
                if(!string.IsNullOrEmpty(tip))
                {
                    (Current.ContentFrame.Content as WatingDialog).TipTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    (Current.ContentFrame.Content as WatingDialog).TipTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }
        public static void Hide()
        {
            Current.Hide();
            Current = null;
        }
    }
}
