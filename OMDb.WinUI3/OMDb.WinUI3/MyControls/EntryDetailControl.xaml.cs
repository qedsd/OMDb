// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

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
using OMDb.WinUI3.ViewModels;
using OMDb.WinUI3.ViewModels.Controls;
using System.Runtime.Serialization.Formatters;
using OMDb.Core.Enums;
using OMDb.WinUI3.Models;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.Utils;
using OMDb.Core.Extensions;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class EntryDetailControl : UserControl
    {
        public ViewModels.EditEntryViewModel VM = new EditEntryViewModel(null);
        public EntryDetailControl()
        {
            this.InitializeComponent();
        }

        private void Button_Path_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
