using Microsoft.UI;
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
using Windows.UI;

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class NotifyPopup : UserControl
    {
        private Popup m_Popup;

        private string m_TextBlockContent;
        private string m_IconContent;
        private Color m_IconColor;
        private TimeSpan m_ShowTime;

        private NotifyPopup()
        {
            this.InitializeComponent();
            m_Popup = new Popup();
            this.Width = MainWindow.Instance.Bounds.Width;
            this.Height = MainWindow.Instance.Bounds.Height;
            m_Popup.Child = this;
            this.Loaded += NotifyPopup_Loaded; ;
            this.Unloaded += NotifyPopup_Unloaded; ;
        }

        public NotifyPopup(string content, string iconStr, Color color, TimeSpan showTime) : this()
        {
            this.m_TextBlockContent = content;
            this.m_IconContent = iconStr;
            this.m_IconColor = color == new Color() ? Colors.IndianRed : color;
            this.m_ShowTime = showTime;
        }

        public NotifyPopup(string content, string iconStr = "\xE10A", Color color = new Color()) : this(content, iconStr, color, TimeSpan.FromSeconds(2))
        {
        }

        public void Show()
        {
            this.m_Popup.IsOpen = true;
        }

        private void NotifyPopup_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbNotify.Text = m_TextBlockContent;
            this.TextBlock_icon.Text = m_IconContent;
            this.TextBlock_icon.Foreground = new SolidColorBrush() { Color = m_IconColor };
            this.sbOut.BeginTime = this.m_ShowTime;
            this.sbOut.Begin();
            this.sbOut.Completed += SbOut_Completed;
            MainWindow.Instance.SizeChanged += Current_SizeChanged; 
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            this.Width = args.Size.Width;
            this.Height = args.Size.Height;
        }

        private void SbOut_Completed(object sender, object e)
        {
            this.m_Popup.IsOpen = false;
        }

        private void NotifyPopup_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.SizeChanged -= Current_SizeChanged;
        }

        public static void ShowSuccess(string text)
        {
            NotifyPopup notifyPopup = new NotifyPopup(text, "\xE082", Colors.MediumSeaGreen);
            notifyPopup.Show();
        }
        public static void ShowError(string text)
        {
            NotifyPopup notifyPopup = new NotifyPopup(text, "\xE171");
            notifyPopup.Show();
        }
    }
}
