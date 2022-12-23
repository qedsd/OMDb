// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.MyControls
{
    public sealed partial class LabelTreeCard : UserControl
    {
        public LabelTreeCard()
        {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty EnrtyStorageProperty = DependencyProperty.Register
            (
            "EnrtyStorage",
            typeof(Models.LabelTree),
            typeof(UserControl),
            new PropertyMetadata(null, new PropertyChangedCallback(SetLabelTree))
            );

        private static void SetLabelTree(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as LabelTreeCard;
            if (card != null)
            {
                card.labelTree = e.NewValue as Models.LabelTree;
                var es = card.labelTree;
            }
        }

        public Models.LabelTree labelTree
        {
            get { return (Models.LabelTree)GetValue(EnrtyStorageProperty); }
            set { SetValue(EnrtyStorageProperty, value); }
        }

    }
}
