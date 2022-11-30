using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OMDb.WinUI3.Wins;
using OMDb.WinUI3.Views.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using OMDb.WinUI3.Models.Tools;

namespace OMDb.WinUI3.Views
{
    public sealed partial class ToolsPage : Page
    {
        public ToolsPage()
        {
            this.InitializeComponent();
            ItemsGridView.ItemsSource = new List<ToolItem>()
            {
                new ToolItem("视频字幕","往视频文件导出、导入字幕",typeof(SubToolPage)),
                new ToolItem("字幕编辑","对字幕文件进行编辑操作",null),
                new ToolItem("字幕翻译","对已有字幕文件进行自动翻译",null),
                new ToolItem("视频转码","",null),
                new ToolItem("音频转码","",null),
                new ToolItem("图片转码","",null),
            };
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ToolItem;
            if(item != null && item.PageType != null)
            {
                var obj = System.Activator.CreateInstance(item.PageType, item.Title);
                var page = obj as ToolPageBase;
                if(page != null)
                {
                    page.Show();
                }
            }
        }
    }
}
