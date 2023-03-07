using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Extensions;
using OMDb.WinUI3.Models;
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
    public sealed partial class LabelPropertyPage : Page
    {
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel();
        public LabelPropertyPage()
        {
            this.InitializeComponent();
            DataContext = VM;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.LP_Baba.Clear();
            foreach (var item in e.AddedItems)
            {
                var lp_Yeye=((LabelPropertyTree)item);
                foreach (var lp in lp_Yeye.Children)
                {
                    VM.LP_Baba.Add(lp);
                }          
            }
        }
    }
}
