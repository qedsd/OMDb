using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OMDb.WinUI3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.TemplateSelectors
{
    internal class LabelCollectionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch(((LabelCollection)item).Template)
            {
                case 1: return Template1;
                case 2: return Template2;
                default: return Template1;
            }
        }
    }
}
