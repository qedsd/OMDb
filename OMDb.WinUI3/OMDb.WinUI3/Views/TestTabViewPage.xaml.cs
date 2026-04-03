using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Interfaces;

namespace OMDb.WinUI3.Views
{
    public sealed partial class TestTabViewPage : Page, ITabViewItemPage
    {
        public TestTabViewPage()
        {
            this.InitializeComponent();
        }

        public string Title => "≤‚ ‘¥ Ãı“≥";

        public void Close()
        {
            
        }
    }
}
