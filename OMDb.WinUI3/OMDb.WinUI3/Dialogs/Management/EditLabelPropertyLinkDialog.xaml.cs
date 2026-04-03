using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.DbModels;
using OMDb.Core.Utils;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.Services.Settings;
using OMDb.WinUI3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class EditLabelPropertyLinkDialog : Page
    {
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel(_labelProperty);

        static LabelProperty _labelProperty=null;
        public EditLabelPropertyLinkDialog(LabelProperty labelProperty)
        {
            this.InitializeComponent();
        }

        public static async Task<List<string>> ShowDialog(LabelProperty labelProperty)
        {
            _labelProperty=labelProperty;
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "关联标签选择";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            EditLabelPropertyLinkDialog content = new EditLabelPropertyLinkDialog(labelProperty);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return null;

            var result = new List<string>();
            var labelPropertySelected= content.VM.LabelPropertyList.Where(a=>a.IsChecked)?.Select(a=>a.LPDb.LPID).ToList();
            return labelPropertySelected;

        }
    }
}
