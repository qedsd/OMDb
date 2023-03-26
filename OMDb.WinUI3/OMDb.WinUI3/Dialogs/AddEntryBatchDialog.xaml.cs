using ColorCode.Compilation.Languages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using OMDb.Core.Extensions;
using OMDb.Core.Services.PluginsService;
using OMDb.WinUI3.Helpers;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.Resource;
using OMDb.WinUI3.Services;
using OMDb.WinUI3.ViewModels;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Net.WebRequestMethods;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.WinUI3.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddEntryBatchDialog : Page
    {
        public AddEntryBatchViewModel VM=new AddEntryBatchViewModel();
        public AddEntryBatchDialog()
        {
            this.InitializeComponent();
            if (PluginsBaseService.EntryInfos.Count() > 0)
            {
                this.ddb.Content = PluginsBaseService.EntryInfos.FirstOrDefault().GetType().Assembly.GetName().Name;
                var mf = new MenuFlyout();
                foreach (var item in PluginsBaseService.EntryInfos)
                {
                    MenuFlyoutItem mfl = new MenuFlyoutItem();
                    mfl.Text = item.GetType().Assembly.GetName().Name;
                    mfl.Click += (s, e) => { this.ddb.Content = mfl.Text; };
                    mf.Items.Add(mfl);
                    mfl.Width = 200;
                }
                this.ddb.Flyout = mf;
            }
            else this.ddb.Content = "无服务";
            /*string xaml = @"<TextBlock Text=""123"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""/>";*/
            string xaml = @"    
    <DataTemplate
        x:Name=""dt""
        x:FieldModifier=""public""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        xmlns:controls=""using:CommunityToolkit.WinUI.UI.Controls""
        xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
        xmlns:local=""using:OMDb.WinUI3.Dialogs""
        xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
        xmlns:models=""using:OMDb.WinUI3.Models""
        xmlns:myControls=""using:OMDb.WinUI3.MyControls"">
        <Grid Height=""100"" HorizontalAlignment=""Left"">
            <StackPanel Orientation=""Horizontal"">
                <Grid>
                    <Image
                        x:Name=""Image_CoverImg""
                        Width=""60""
                        Height=""80""
                        Stretch=""Fill"" />
                    <Button
                        Width=""60""
                        Height=""80""
                        VerticalAlignment=""Center""
                        Background=""Transparent"" />
                </Grid>
                <StackPanel>
                    <StackPanel Orientation=""Horizontal"">
                        <TextBlock
                            Width=""70""
                            Margin=""8,5,0,0""
                            Text=""詞條名称："" />
                        <TextBlock
                            Width=""100""
                            Margin=""0,5,0,0""
                            Text=""111"">
                        </TextBlock>
                        <TextBlock Margin=""8,5,0,0"" Text=""詞條地址："" />
                        <TextBlock
                            Width=""500""
                            Margin=""0,5,0,0""
                            Text=""111"" />
                    </StackPanel>
                    <StackPanel Orientation=""Horizontal"">
                        <TextBlock
                            Width=""70""
                            Margin=""8,5,0,0""
                            Text=""發行日期："" />
                        <TextBlock x:Name=""ReleaseDate"" Width=""100"" Margin=""0,5,0,0"">
                        </TextBlock>
                        <TextBlock
                            Width=""70""
                            Margin=""8,5,0,0""
                            Text=""分類："" />
                        <TextBlock Width=""180"" Margin=""0,5,0,0"">
                        </TextBlock>
                    </StackPanel>
                    <StackPanel x:Name=""sp_lp"" Orientation=""Horizontal"">
                        <TextBlock
                            Width=""70""
                            Margin=""8,5,0,0""
                            Text=""主演："" />
                        <TextBlock Width=""100"" Margin=""0,5,0,0"">
                        </TextBlock>
                    </StackPanel>
                    <StackPanel Orientation=""Horizontal"">
                        <RatingControl
                            Margin=""8,2,0,0""
                            HorizontalAlignment=""Right""
                            Value=""5"" />
                    </StackPanel>
                </StackPanel>
            </StackPanel>
        </Grid>
    </DataTemplate>
";
            var stringReader = new StringReader(xaml);
            XmlReader xr = XmlReader.Create(stringReader);
            DataTemplate dataTemplate = (DataTemplate)XamlReader.Load(xaml);
            this.lv_edc.ItemTemplate = dataTemplate;
        }

        public static async Task<string> ShowDialog()
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = "词条批量新增";
            dialog.PrimaryButton.Content = "确认";
            dialog.CancelButton.Content = "取消";
            AddEntryBatchDialog content = new AddEntryBatchDialog();
            dialog.ContentFrame.Content = content;
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                return null;
            }
            else
            {
                return null;
            }
        }

        private void SelectFolders_Click(object sender, RoutedEventArgs e)
        {
            var path = VM.SelectedEnrtyStorage.StoragePath;
            this.ei.treeFolders.ItemsSource= FileHelper.FindFolderItems(path);
            this.flyGrid.Visibility= Visibility.Visible;
        }

        private void ReturnFolder_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in (List<string>)this.eic.ItemsSource)
            {
                EntryDetail ed = new EntryDetail();
                ed.FullCoverImgPath = CommonService.GetCoverByPath(item);
                VM.EntryDetailCollection.Add(ed);
            }
            this.ei.treeFolders.SelectedItems.Clear();
            this.btn_FolderPicker.Flyout.Hide();

            var edc_it=lv_edc.ItemTemplate;
            var edc_ict = lv_edc.ItemContainerTransitions;
            var edc_i = lv_edc.Items;
            var edc_dmp = lv_edc.DisplayMemberPath;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ei.treeFolders.SelectedItems.Clear();
            this.btn_FolderPicker.Flyout.Hide();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            List<string> lstEic= new List<string>();
            foreach (var item in this.ei.treeFolders.SelectedItems)
            {
                var ei = (ExplorerItem)item;
                lstEic.Add(ei.FullName);

            }
            this.eic.ItemsSource = lstEic;
            this.ei.treeFolders.SelectedItems.Clear();
            
        }
    }
}
