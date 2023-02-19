using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
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
using static System.Net.WebRequestMethods;

namespace OMDb.WinUI3.Dialogs
{
    public sealed partial class EditEntryDialog : Page
    {
        public ViewModels.EditEntryViewModel VM { get; set; }
        public EditEntryDialog(Core.Models.Entry entry)
        {
            VM = new ViewModels.EditEntryViewModel(entry);
            this.InitializeComponent();
            //封面不为空 ->设置封面
            if (entry != null && entry.CoverImg != null) { Image_CoverImg.Source = new BitmapImage(new Uri(VM.Entry.CoverImg)); }

            //取属性标签
            var lst_label_property = Core.Services.LabelService.GetAllLabel(DbSelectorService.dbCurrentId, true);
            if (lst_label_property.Count > 0) VM.Label_Property = new List<Models.Label>();
            {
                foreach (var item in lst_label_property) { VM.Label_Property.Add(new Models.Label(item)); }
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
                    Grid grid = new Grid()
                    {
                        Margin = new Thickness(0, 3, 0, 0),
                        RenderTransform = new CompositeTransform() { ScaleX = 0.75, ScaleY = 0.75 }
                    };


                    /*Button btn = new Button()
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(0, 5, 0, 0),
                        Content = new SymbolIcon(Symbol.Find),
                        RenderTransform = new CompositeTransform() { ScaleX = 0.92, ScaleY = 0.92 }
                    };
                    //var lstSon = VM.Label_Property.Where(a => a.LabelDb.ParentId.NotNullAndEmpty()).Where(a => item.LabelDb.Id == a.LabelDb.ParentId).Select(a => a.LabelDb.Name).ToList();
                    
                    Flyout flyout = new Flyout();

                    flyout.Content = new MyControls.LabelsControl()
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Labels = lstSon,
                        Mode = LabelControlMode.Add,
                    };*/
                    var lstSon = VM.Label_Property.Where(a => a.LabelDb.ParentId.NotNullAndEmpty()).Where(a => item.LabelDb.Id == a.LabelDb.ParentId);
                    var btn=new MyControls.LabelsControl()
                    {
                        HorizontalAlignment= HorizontalAlignment.Right,
                        Labels = lstSon,
                        Mode = LabelControlMode.Add,
                    };

                    var binding = new Binding()
                    {
                        ElementName = "ListView",
                        Mode = BindingMode.OneWay,
                        Path = new PropertyPath("SelectedItems"),
                    };
                    TextBox tBox = new TextBox()
                    {
                        Width = 220,
                        IsReadOnly = true,
                    };
                    tBox.SetBinding(TextBox.TextProperty, binding);
                    grid.Children.Add(tBox);
                    grid.Children.Add(btn);
                    stack.Children.Add(grid);

                    stp.Children.Add(stack);
                }
            }
        }

        //private void RadioButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var selected = ComboBox_Names.SelectedItem as Models.EntryName;
        //    VM.EntryNames.ForEach(p =>
        //    {
        //        if (p.IsDefault && p != selected)
        //        {
        //            p.IsDefault = false;
        //        }
        //    });
        //}
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
