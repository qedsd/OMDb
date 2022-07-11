using Microsoft.UI.Xaml;
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
    public sealed partial class MsgDialog : Page
    {
        public MsgDialog(string content)
        {
            this.InitializeComponent();
            TextBlock_Content.Text = content;
        }
        public static async Task ShowDialog(string content, string title = "提示")
        {
            MyContentDialog dialog = new MyContentDialog();
            dialog.TitleTextBlock.Text = title;
            dialog.PrimaryButton.Visibility = Visibility.Collapsed;
            dialog.CancelButton.Content = "关闭";
            dialog.ContentFrame.Content = new MsgDialog(content);
            await dialog.ShowAsync();
        }
    }
}
