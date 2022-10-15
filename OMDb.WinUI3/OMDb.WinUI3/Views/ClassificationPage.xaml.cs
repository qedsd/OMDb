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

namespace OMDb.WinUI3.Views
{
    public sealed partial class ClassificationPage : Page
    {
        public ClassificationPage()
        {
            this.InitializeComponent();
            SizeChanged += ClassificationPage_SizeChanged;
        }

        private void ClassificationPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Banner.Height = this.ActualHeight * 0.8;
        }
    }
}
