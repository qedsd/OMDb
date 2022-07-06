using OMDb.Core.Interfaces;
using OMDb.Core.Models;
using System.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OMDb.IMDb
{
    [Export(typeof(IRate))]
    public class Rate : IRate
    {
        Rating IRate.Rate()
        {
            return new Rating(9, 10);
        }
    }
}
