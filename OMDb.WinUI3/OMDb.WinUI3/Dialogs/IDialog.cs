using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Dialogs
{
    internal interface IDialog
    {
        void Show();
        void Close();
    }
}
