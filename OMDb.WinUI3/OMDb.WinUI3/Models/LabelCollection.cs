using Microsoft.UI.Xaml.Media;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    internal class LabelCollection
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Entry> Entries { get; set; }
        public ImageSource ImageSource { get; set; }
        public int Template { get; set; } = 1;
    }
}
