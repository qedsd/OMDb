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

namespace OMDb.WinUI3.Views
{
    public sealed partial class EntryHomePage : Page
    {
        public ViewModels.EntryHomeViewModel VM { get; set; }
        public EntryHomePage()
        {
            this.InitializeComponent();
            VM = new ViewModels.EntryHomeViewModel();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter != null)
            {
                var dbid = e.Parameter as string;
                var target = VM.EntryStorages.FirstOrDefault(p=>p.StorageName == dbid);
                if(target != null)
                {
                    foreach(var item in VM.EntryStorages)
                    {
                        if(item != target)
                        {
                            item.IsChecked = false;
                        }
                        else
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            }
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            VM.QuerySubmittedCommand?.Execute(args.ChosenSuggestion);
        }
    }
}
