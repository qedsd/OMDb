using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace OMDb.WinUI3.Converters
{
    public sealed class BoolReverseConverte : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
