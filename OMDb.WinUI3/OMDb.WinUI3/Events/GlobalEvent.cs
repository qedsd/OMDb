using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Events
{
    public static class GlobalEvent
    {
        public static event EventHandler<EntryEventArgs> AddEntryEvent;

        public static event EventHandler<EntryEventArgs> RemoveEntryEvent;
        public static event EventHandler<EntryEventArgs> UpdateEntryEvent;

        /// <summary>
        /// 最近观看文件更新通知事件
        /// </summary>
        public static event EventHandler<RecentFileChangedEventArgs> RecentFileChangedEvent;

        public static void NotifyRemoveEntry(object sender, EntryEventArgs args)
        {
            RemoveEntryEvent?.Invoke(sender, args);
        }
        public static void NotifyAddEntry(object sender, EntryEventArgs args)
        {
            AddEntryEvent?.Invoke(sender, args);
        }
        public static void NotifyUpdateEntry(object sender, EntryEventArgs args)
        {
            UpdateEntryEvent?.Invoke(sender, args);
        }
        public static void NotifyRecentFileChanged(object sender, RecentFileChangedEventArgs args)
        {
            RecentFileChangedEvent?.Invoke(sender, args);
        }
    }
}
