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

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class WaitingPopup : UserControl
    {
        private Popup Popup;
        /// <summary>
        /// 不可用
        /// </summary>
        public WaitingPopup()
        {
            this.InitializeComponent();
            Popup = new Popup();
            Popup.Child = this;
            this.Width = Helpers.WindowHelper.MainWindow.Bounds.Width;
            this.Height = Helpers.WindowHelper.MainWindow.Bounds.Height;
            Helpers.WindowHelper.MainWindow.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            this.Width = args.Size.Width;
            this.Height = args.Size.Height;
        }

        private static WaitingPopup Instance;
        public static void Show()
        {
            if (Instance == null)
            {
                Instance = new WaitingPopup();
            }
            Instance.ProgressRing.IsActive = true;
            Instance.Popup.IsOpen = true;
        }
        public static void Hide()
        {
            if (Instance != null)
            {
                Instance.ProgressRing.IsActive = false;
                Instance.Popup.IsOpen = false;
                Instance = null;
            }
        }
    }
}
