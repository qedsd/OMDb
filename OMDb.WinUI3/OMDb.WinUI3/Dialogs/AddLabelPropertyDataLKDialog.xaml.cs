using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NPOI.POIFS.Properties;
using OMDb.Core.DbModels;
using OMDb.Core.Services;
using OMDb.Core.Utils;
using OMDb.Core.Utils.Extensions;
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
    public sealed partial class EditLabelPropertyDataLinkDialog : Page
    {
        static string parentId = string.Empty;
        public ViewModels.LabelPropertyViewModel VM { get; set; } = new ViewModels.LabelPropertyViewModel(parentId);
        public EditLabelPropertyDataLinkDialog(LabelPropertyDb labelPropertyDb)
        {
            this.InitializeComponent();
            var linkIdList=Core.Services.LabelPropertyService.GetLinkId(DbSelectorService.dbCurrentId, labelPropertyDb.LPID);
            var linkDbList=Core.Services.LabelPropertyService.GetLabels(linkIdList);
            linkDbList.ForEach(a => VM.DtData.Add(a));
        }

        public static async Task<List<string>> ShowDialog(LabelPropertyDb labelPropertyDb)
        {
            parentId = labelPropertyDb.ParentId;
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "关联标签数据选择";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            EditLabelPropertyDataLinkDialog content = new EditLabelPropertyDataLinkDialog(labelPropertyDb);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
                return null;   

            List<string> result = new List<string>();
            //数据库新增属性&属性数据 合并返回
            foreach (var item in content.VM.DtData)
            {
                if (!item.LPID.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                {
                    result.Add(item.LPID);
                    continue;
                }

                if (item.Name.IsNullOrEmptyOrWhiteSpazeOrCountZero())//属性标签数据名称为空
                    continue;

                //手动输入的属性数据

                var childs = content.VM.LabelPropertyTrees.Where(a => a.LabelProperty.LPDb.LPID.Equals(item.ParentId)).FirstOrDefault().Children;
                var lp_repeat = childs.Where(a => a.LabelProperty.LPDb.Name.Equals(item.Name));
                //手输的属性数据是原有的：
                if (lp_repeat.Count() > 0)
                {
                    result.Add(lp_repeat.FirstOrDefault().LabelProperty.LPDb.LPID);
                    continue;
                }

                //手输新增
                item.LPID = Guid.NewGuid().ToString();
                item.DbCenterId = DbSelectorService.dbCurrentId;
                Core.Services.LabelPropertyService.AddLabel(item);
                var lpt = new LabelPropertyTree(item);
                content.VM.LabelPropertyTrees.Where(a => a.LabelProperty.LPDb.LPID.Equals(item.ParentId)).FirstOrDefault().Children.Add(lpt);
                result.Add(item.LPID);
            }
            return result;
        }

        private void ImportLink_Click(object sender, RoutedEventArgs e)
        {
            var items = this.GridView_Current_LPEZCollection.SelectedItems;
            var itemsRoot = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            foreach (var item in items)
            {
                var lpt = item as LabelPropertyTree;
                VM.DtData.Add(lpt.LabelProperty.LPDb);
            }
        }

        private void BroomLink_Click(object sender, RoutedEventArgs e)
        {
            VM.DtData.Clear();
        }

        private void AddLink_Click(object sender, RoutedEventArgs e)
        {
            var lp = new LabelPropertyDb();
            var itemsRoot = (LabelPropertyTree)this.ListView_LabelPropertyTrees.SelectedItem;
            if (itemsRoot.IsNullOrEmptyOrWhiteSpazeOrCountZero()) return;
            lp.ParentId = itemsRoot.LabelProperty.LPDb.LPID;
            VM.DtData.Add(lp);
        }

        private void RemoveLink_Click(object sender, RoutedEventArgs e)
        {
            var items = this.Link_Table.SelectedItems.ToList();
            foreach (var item in items)
            {
                var lp = (LabelPropertyDb)item;
                var result = VM.DtData.Remove(lp);
            }
        }
    }
}
