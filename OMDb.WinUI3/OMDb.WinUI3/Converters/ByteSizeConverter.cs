using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class ByteSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double size_byte = ((long)value);
            if (size_byte < 1024)
            {
                return $"{size_byte}B";
            }
            else if (size_byte < 1048576)
            {
                if (size_byte % 1024 == 0)
                {
                    return $"{size_byte / 1024}KB";
                }
                else
                {
                    return $"{size_byte / 1024:F2}KB";
                }
            }
            else if (size_byte < 1073741824)
            {
                if (size_byte % 1048576 == 0)
                {
                    return $"{size_byte / 1048576}MB";
                }
                else
                {
                    return $"{size_byte / 1048576:F2}MB";
                }
            }
            else
            {
                if (size_byte % 1073741824 == 0)
                {
                    return $"{size_byte / 1073741824}GB";
                }
                else
                {
                    return $"{size_byte / 1073741824:F2}GB";
                }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
