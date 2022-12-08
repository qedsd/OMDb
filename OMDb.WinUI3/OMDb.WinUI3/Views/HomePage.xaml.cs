using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
    public sealed partial class HomePage : Page
    {
        public ViewModels.HomeViewModel VM { get; set; } = new ViewModels.HomeViewModel();
        public HomePage()
        {
            this.InitializeComponent();
            Img.Source = new BitmapImage(new Uri(@"D:\OMDb\图片\p2879947762.jpg"));
        }
    }
}
