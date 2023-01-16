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
using System.Text;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace OMDb.WinUI3.Views
{
    public sealed partial class LabelPage : Page
    {
        public ViewModels.LabelViewModel VM { get; set; } = new ViewModels.LabelViewModel();
        public LabelPage()
        {
            this.InitializeComponent();
            DataContext = VM;
        }


        private void StackPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }

        private void StackPanel_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        private void Combo2_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
