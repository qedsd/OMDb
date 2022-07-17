using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace OMDb.WinUI3.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsReverse { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if ((bool)value)
            {
                return IsReverse ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return IsReverse ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
