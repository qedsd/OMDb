using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Models
{
    public class HomeItemConfig
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public HomeItemConfig() { }
        public HomeItemConfig(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
