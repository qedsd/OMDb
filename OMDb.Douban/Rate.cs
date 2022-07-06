using OMDb.Core.Interfaces;
using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Douban
{
    [Export(typeof(IRate))]
    public class Rate : IRate
    {
        Rating IRate.Rate(string id)
        {
            return new Rating(3, 5,"douban");
        }
    }
}
