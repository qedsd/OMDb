using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Events
{
    public class EntryEventArgs:EventArgs
    {
        public EntryEventArgs()
        {

        }
        public EntryEventArgs(Entry entry)
        {
            Entry = entry;
        }
        public Entry Entry { get; set; }
    }
}
