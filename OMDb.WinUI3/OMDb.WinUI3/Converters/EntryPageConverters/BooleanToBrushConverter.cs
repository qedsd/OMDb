using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Converters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isCheck = (bool)value;
            if (isCheck)
            {
                return new SolidColorBrush(Colors.Green); // Set your desired background color when IsCheck is true
            }
            else
            {
                return new SolidColorBrush(Colors.Red); // Set your desired background color when IsCheck is false
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
