using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.DbModels;
using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditLabelDialog : Page
    {
        private Core.DbModels.LabelClassDb Label;
        public EditLabelDialog(bool IsRoot, Core.DbModels.LabelClassDb label)
        {
            this.InitializeComponent();
            if (label == null)
            {
                Label = new Core.DbModels.LabelClassDb();

                if (IsRoot)
                {
                    IsProperty.Visibility = Visibility.Visible;
                    IsShowOnClassificationPage.Visibility = Visibility.Visible;
                    IsProperty.IsOn = false;
                    IsShowOnClassificationPage.IsOn = false;
                }
                else 
                {
                    IsProperty.Visibility= Visibility.Collapsed;
                    IsShowOnClassificationPage.Visibility= Visibility.Collapsed;
                }
            }
            else
            {
                Label = label;
                TextBox_Name.Text = label.Name;
                TextBox_Desc.Text = label.Description;
                if (IsRoot)
                {
                    IsProperty.Visibility = Visibility.Visible;
                    IsShowOnClassificationPage.Visibility = Visibility.Visible;
                    IsShowOnClassificationPage.IsOn = label.IsShow;
                }
                else
                {
                    IsProperty.Visibility = Visibility.Collapsed;
                    IsShowOnClassificationPage.Visibility = Visibility.Collapsed;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsRoot"></param>大标签
        /// <param name="label"></param>
        /// <returns></returns>
        public static async Task<Core.DbModels.LabelClassDb> ShowDialog(bool IsRoot, Core.DbModels.LabelClassDb label = null)
        {
            bool IsNew = (label == null);
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = IsNew ? "新建标签" : "编辑标签";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditLabelDialog content = new EditLabelDialog(IsRoot, label);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                content.Label.Name = content.TextBox_Name.Text;
                content.Label.Description = content.TextBox_Desc.Text;
                content.Label.DbCenterId = DbSelectorService.dbCurrentId;
                content.Label.IsShow = content.IsShowOnClassificationPage.IsOn;
                return content.Label;
            }
            else
            {
                return null;
            }
        }
    }
}
