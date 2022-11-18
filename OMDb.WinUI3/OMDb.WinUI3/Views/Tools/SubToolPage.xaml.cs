using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.ViewModels.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Views.Tools
{
    public sealed partial class SubToolPage : Page
    {
        public SubToolPage()
        {
            this.InitializeComponent();
            this.SizeChanged += SubToolPage_SizeChanged;
        }

        private void SubToolPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdataExpander();
        }

        private void UpdataExpander()
        {
            InnerSubExpander.MaxHeight = this.ActualHeight / 2;
        }

        private void NewSubExpander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            UpdataExpander();
        }

        private void InnerSubExpander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            UpdataExpander();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SubToolViewModel).RemoveSubCommand.Execute(NewSubListView.SelectedItems);
        }
    }
}
