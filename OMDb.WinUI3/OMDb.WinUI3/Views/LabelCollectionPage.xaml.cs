using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.Models;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views
{
    public sealed partial class LabelCollectionPage : Page
    {
        private LabelCollectionViewModel VM;
        public LabelCollectionPage()
        {
            this.InitializeComponent();
            VM = new LabelCollectionViewModel();
            DataContext = VM;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var collection = e.Parameter as LabelCollection;
            VM.Entries = collection?.Entries;
            VM.Title = collection?.Title;
            VM.Description = collection?.Description;
            VM.LabelId = collection?.Id;
        }
    }
}
