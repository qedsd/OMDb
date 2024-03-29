﻿using Microsoft.UI.Xaml;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditDbCenter : Page
    {
        public EditDbCenter(string dbName = null)
        {
            this.InitializeComponent();
        }

        public static async Task<string> ShowDialog(string dbName = null)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = dbName == null ? "新增数据中心" : "编辑数据中心";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditDbCenter content = new EditDbCenter(dbName);
            dialog.ContentFrame.Content = content;
            content.DbName.Text = dbName;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                dbName = content.DbName.Text;
                return dbName;
            }
            else
            {
                return "04833378-22bb-465b-9582-fb1bab622de";
            }
        }
    }
}
