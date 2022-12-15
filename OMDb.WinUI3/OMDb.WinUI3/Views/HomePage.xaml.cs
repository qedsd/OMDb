using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.MyControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class HomePage : Page
    {
        public ViewModels.HomeViewModel VM { get; set; } = new ViewModels.HomeViewModel();
        public HomePage()
        {
            this.InitializeComponent();
            SizeChanged += ClassificationPage_SizeChanged;
            ClassificationPage_SizeChanged(null, null);
        }
        private void ClassificationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CoverLineGrid.Height = this.ActualHeight * 0.8;
        }
    }
}
