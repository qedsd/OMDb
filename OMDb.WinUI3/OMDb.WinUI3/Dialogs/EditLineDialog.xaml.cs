using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditLineDialog : DialogBase
    {
        public string Line
        {
            get => InputTextBox.Text;
        }
        public EditLineDialog(Core.Models.ExtractsLineBase line)
        {
            this.InitializeComponent();
            if(line == null)
            {
                TitleTextBlock.Text = "新摘录台词";
            }
            else
            {
                TitleTextBlock.Text = "编辑摘录台词";
                InputTextBox.Text = line.Line;
            }
        }
        private bool clickConfirm = false;

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            clickConfirm = false;
            Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            clickConfirm = true;
            Close();
        }

        public override async Task<bool> ShowAsync()
        {
            await base.ShowAsync();
            return clickConfirm;
        }
    }
}
