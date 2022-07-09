using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class FileToDirConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return System.IO.Path.GetDirectoryName(value.ToString());
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
