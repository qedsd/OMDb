using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using OMDb.Core.Utils;
using OMDb.WinUI3.Services;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class LPIdToReadOnlyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value.IsNullOrEmptyOrWhiteSpace())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
