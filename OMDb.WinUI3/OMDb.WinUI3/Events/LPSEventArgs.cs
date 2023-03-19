using OMDb.Core.Models;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Events
{
    public class LPSEventArgs:EventArgs
    {
        public LPSEventArgs()
        {

        }
        public LPSEventArgs(List<Models.LabelProperty> lps)
        {
            LPS = lps;
        }
        public List<Models.LabelProperty> LPS { get; set; }

        public static event EventHandler UpdateLPS;
    }
}
