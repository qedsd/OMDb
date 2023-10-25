using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using OMDb.Core.DbModels;
using OMDb.Core.Models;
using OMDb.Core.Utils;
using OMDb.Core.Utils.Extensions;
using OMDb.WinUI3.Models;
using OMDb.WinUI3.MyControls;
using OMDb.WinUI3.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OMDb.WinUI3.ViewModels
{
    public partial class EntryHomeViewModel: ObservableObject
    {
        #region ICommand
        public ICommand RefreshCommand => new RelayCommand(() =>
        {
            Init();
            Helpers.InfoHelper.ShowMsg("刷新完成");
        });
        public ICommand ItemClickCommand => new RelayCommand<Core.Models.Entry>((entry) =>
        {
            TabViewService.AddItem(new Views.EntryDetailPage(entry));
        });
        public ICommand LabelChangedCommand => new RelayCommand<IEnumerable<Models.LabelClass>>((items) =>
        {
            _ = UpdateEntryListAsync();
        });
        public ICommand EntryStorageChangedCommand => new RelayCommand<IEnumerable<Models.EnrtyStorage>>((items) =>
        {
            _ = UpdateEntryListAsync();
        });
        public ICommand QuerySubmittedCommand => new RelayCommand<Core.Models.QueryResult>(async (item) =>
        {
            if (item == null)
            {
                if (string.IsNullOrEmpty(AutoSuggestText))
                {
                    _ = UpdateEntryListAsync();
                }
                else
                {
                    List<Core.Models.Entry> items = new List<Core.Models.Entry>();
                    foreach (var p in AutoSuggestItems.GroupBy(p => p.DbId))
                    {
                        var entryIds = p.Select(p => p.Id).ToList().Distinct();
                        items.AddRange(await Core.Services.EntryService.GetEntryByIdsAsync(entryIds, p.Key));
                    }
                    Helpers.WindowHelper.MainWindow.DispatcherQueue.TryEnqueue(() =>
                    {
                        Entries = items.ToObservableCollection();
                    });
                }
            }
            else
            {
                AutoSuggestItem = item;
            }
        });
        public ICommand AddEntryCommand => new RelayCommand(async () =>
        {
            Services.ConfigService.LoadStorages();
            if (Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                await Services.EntryService.AddEntryAsync();
            }
        });
        public ICommand AddEntryBatchCommand => new RelayCommand(async () =>
        {
            Services.ConfigService.LoadStorages();
            if (Services.ConfigService.EnrtyStorages.Count == 0)
            {
                await Dialogs.MsgDialog.ShowDialog("请先创建仓库");
            }
            else
            {
                await Services.EntryService.AddEntryBatchAsync();
            }
        });

        public ICommand AddEntrySortInfoCommand => new RelayCommand(() =>
        {
            if (EntrySortInfoCurrent == null) return;
            if (EntrySortInfoCurrent.ParentTag == null) return;
            if (EntrySortInfoResults.Select(a=>a.Title).Contains(EntrySortInfoCurrent.Title)&& EntrySortInfoResults.Select(a => a.ESIT.ParentTag).Contains(EntrySortInfoCurrent.ParentTag))
                return;
            EntrySortInfoResult ESIR = new EntrySortInfoResult(EntrySortInfoCurrent);
            EntrySortInfoResults.Add(ESIR);
        });
        public ICommand RemoveEntrySortInfoCommand => new RelayCommand(() =>
        {
            EntrySortInfoResults.Remove(EntrySortInfoResultCurrent);
        });
        public ICommand ClearEntrySortInfoCommand => new RelayCommand(() =>
        {
            EntrySortInfoResults.Clear();
        });

        public ICommand ConfirmSortCommand => new RelayCommand<Flyout>((flyoutParameter) =>
        {
            //排序逻辑
            var sb2 = LabelClassTrees;
            Flyout flyout = (Flyout)flyoutParameter;
            flyout.Hide();
        });

        public ICommand CancelSortCommand => new RelayCommand<Flyout>((flyoutParameter) =>
        {
            Flyout flyout = (Flyout)flyoutParameter;
            flyout.Hide();
            EntrySortInfoResults.Clear();
        });

        public ICommand ConfirmFilterCommand => new RelayCommand<Flyout>((flyoutParameter) =>
        {
            var lct = this.LabelClassTrees;
            var lpt = this.LabelPropertyTrees;

            Flyout flyout = (Flyout)flyoutParameter;
            flyout.Hide();
            EntrySortInfoResults.Clear();
        });

        public ICommand CancelFilterCommand => new RelayCommand<Flyout>((flyoutParameter) =>
        {
            Flyout flyout = (Flyout)flyoutParameter;
            flyout.Hide();
            EntrySortInfoResults.Clear();
        });


        #endregion
    }
}
