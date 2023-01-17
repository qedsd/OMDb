using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Windows.UI;

namespace OMDb.WinUI3.Converters
{
    public sealed class StringToImgSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is String)
                {
                    var bi = new BitmapImage(new Uri(value.ToString()));
                    return bi;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is String)
                {
                    var bi = new BitmapImage(new Uri(value.ToString()));
                    return bi;
                }
                else return null;
            }
            else
            {
                return null;
            }
        }
    }
}
