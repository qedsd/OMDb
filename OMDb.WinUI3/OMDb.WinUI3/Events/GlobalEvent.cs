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
    }
}
