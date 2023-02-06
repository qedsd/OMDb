using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using OMDb.WinUI3.Services;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class DbSourceToBool : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString() == Services.Settings.DbSelectorService.dbCurrent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
