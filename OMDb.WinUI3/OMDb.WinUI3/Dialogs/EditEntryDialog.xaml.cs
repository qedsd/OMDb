using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using OMDb.Core.DbModels;
using OMDb.Core.Extensions;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static OMDb.WinUI3.MyControls.LabelsControl;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryViewModel VM { get; set; }
        public EditEntryDialog(Core.Models.Entry entry)
        {
            VM = new ViewModels.EditEntryViewModel(entry);
            this.InitializeComponent();

            //取属性标签
            var lst_label_property = Core.Services.LabelService.GetAllLabel(DbSelectorService.dbCurrentId, true);

            var labelIds = new List<string>();
            if (entry != null) labelIds = Core.Services.LabelService.GetLabelIdsOfEntry(entry.EntryId);
            if (lst_label_property.Count > 0) VM.Label_Property = new List<Models.Label>();
            {
                foreach (var item in lst_label_property)
                {
                    var label = new Models.Label(item);
                    if (labelIds.Count > 0 && labelIds.Contains(item.Id)) label.IsChecked = true;
                    VM.Label_Property.Add(label);
                }
                var lstBaba = VM.Label_Property.Where(a => !a.LabelDb.ParentId.NotNullAndEmpty()).DepthClone<List<Models.Label>>();
                foreach (var item in lstBaba)
                {
                    StackPanel stack = new StackPanel();
                    stack.Orientation = Orientation.Horizontal;
                    //属性 -> 标题
                    TextBlock tBlock = new TextBlock()
                    {
                        Margin = new Thickness(8, 5, 0, 0),
                        Text = item.LabelDb.Name + "：",
                        Width = 87
                    };
                    stack.Children.Add(tBlock);
                    //属性 -> 属性
                    var lbc = new LabelsControl2();
                    lbc.Labels = VM.Label_Property.Where(a => a.LabelDb.ParentId.NotNullAndEmpty()).Where(a => item.LabelDb.Id == a.LabelDb.ParentId);
                    stack.Children.Add(lbc);
                    stp.Children.Add(stack);
                }
            }


            //封面不为空 -> 设置封面
            if (entry != null && entry.CoverImg != null) { Image_CoverImg.Source = new BitmapImage(new Uri(VM.Entry.CoverImg)); }
            if (entry != null)
            {
                switch (entry.SaveType)
                {
                    case "1":
                        this.SetFolder.IsChecked = true;
                        /*if (!(VM.EntryDetail.PathFolder==null)&& VM.EntryDetail.PathFolder.Length>0)
                        {
                            PointFolder.Text = VM.EntryDetail.PathFolder;
                        }*/
                        break;
                    case "2": 
                        this.SetFile.IsChecked = true;                     
                        break;
                    case "3": 
                        this.Local.IsChecked = true;
                        break;
                    default:
                        this.SetFolder.IsChecked = true;
                        break; 
                }
            }
            else
            {
                this.SetFolder.IsChecked = true;
            }




        }
        /// <summary>
        /// 返回的entry的Path、CoverImg都为全路径
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<Models.EntryDetail> ShowDialog(Core.Models.Entry entry = null)
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = entry == null ? "新建词条" : "编辑词条";
            dialog.PrimaryButton.Content = "保存";
            dialog.CancelButton.Content = "取消";
            EditEntryDialog content = new EditEntryDialog(entry);
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (entry == null)
                {
                    entry = content.VM.Entry;
                    //entry.Name = content.VM.EntryNames.FirstOrDefault(p => p.IsDefault)?.Name;
                    entry.Name = content.VM.EntryName;
                    entry.DbId = content.VM.SelectedEnrtyStorage?.StorageName;
                }
                else
                {
                    entry.CopyFrom(content.VM.Entry);
                    //entry.Name = content.VM.EntryNames.FirstOrDefault(p => p.IsDefault)?.Name;
                    entry.Name = content.VM.EntryName;
                }
                var entryDetail = new Models.EntryDetail()
                {
                    Entry = entry,
                    //Names = content.VM.EntryNames.ToObservableCollection(),
                    Labels = content.VM.Labels.Select(p => p.LabelDb).ToList(),
                    FullCoverImgPath = content.VM.Entry.CoverImg,
                    FullEntryPath = content.VM.Entry.Path,
                };
                entryDetail.Entry.CoverImg = Path.Combine(Services.ConfigService.InfoFolder, Path.GetFileName(entry.CoverImg));
                entryDetail.Entry.Path = Helpers.PathHelper.EntryRelativePath(entry);

                //关于 标签 -> 属性 的保存
                var lst = content.VM.Label_Property.Where(a => a.IsChecked == true).Select(a => a.LabelDb).ToList();
                entryDetail.Labels.AddRange(lst);
                //存储方式
                entryDetail.SaveType= content.SetFolder.IsChecked==true? "1" :content.SetFile.IsChecked==true? "2" :"3";
                //存储地址


                return entryDetail;
                //return new Tuple<Core.Models.Entry, List<Models.EntryName>>(entry,content.VM.EntryNames);
            }
            else
            {
                return null;
            }
        }

        private async void Button_Path_Click(object sender, RoutedEventArgs e)
        {
            var folder = await Helpers.PickHelper.PickFolderAsync();
            if (folder != null)
            {
                if (!folder.Path.StartsWith(Path.GetDirectoryName(VM.SelectedEnrtyStorage.StoragePath), StringComparison.OrdinalIgnoreCase))
                {
                    Helpers.DialogHelper.ShowError("请选择位于仓库下的路径");
                }
                else
                {
                    VM.PointFolder = folder.Path;
                    VM.EntryName = folder.Path.SubString_A21("\\", 1, false, false);
                }
            }
        }

        private async void Button_CoverImg_Click(object sender, RoutedEventArgs e)
        {
            var file = await Helpers.PickHelper.PickImgAsync();
            if (file != null)
            {
                VM.Entry.CoverImg = file.Path;
                Image_CoverImg.Source = new BitmapImage(new Uri(file.Path));
            }
        }
        /// <summary>
        /// 编辑词条名的时候同步修改仓库路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (VM.EntryName.IsDefault)
            //{
            //    VM.SetEntryPath((sender as TextBox).Text);
            //}
            VM.SetEntryPath((sender as TextBox).Text);
        }
    }
}
