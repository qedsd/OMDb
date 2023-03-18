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
    public sealed partial class EditLabelPropertyDialog : Page
    {
        private Core.DbModels.LabelPropertyDb Label;
        public EditLabelPropertyDialog(Core.DbModels.LabelPropertyDb label)
        {
            this.InitializeComponent();

            if (label != null)
            {
                Label = label;
                this.TextBox_Name.Text = label.Name;
                TextBox_Desc.Text = label.Description;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IsRoot"></param>大标签
        /// <param name="label"></param>
        /// <returns></returns>
        public static async Task<Core.DbModels.LabelPropertyDb> ShowDialog(Core.DbModels.LabelPropertyDb label = null)
        {
            var IsNew = (bool)(label == null);
            if (IsNew) label=new LabelPropertyDb();
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = IsNew ? "新建标签" : "编辑标签";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditLabelPropertyDialog content = new EditLabelPropertyDialog(label);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                content.Label.Name = content.TextBox_Name.Text;
                content.Label.Description = content.TextBox_Desc.Text;
                content.Label.DbSourceId = DbSelectorService.dbCurrentId;
                return content.Label;
            }
            else
            {
                return null;
            }
        }
    }
}
