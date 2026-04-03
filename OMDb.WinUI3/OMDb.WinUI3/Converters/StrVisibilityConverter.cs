using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class StrVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                switch (value.ToString())
                {
                    case "Visible": return Visibility.Visible;
                    case "Collapsed": return Visibility.Collapsed;
                    default: return Visibility.Visible;
                }
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
