using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using OMDb.WinUI3.Services;
using System;


namespace OMDb.WinUI3.Converters
{
    public sealed class LPIdToLPNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var lpdb=Core.Services.LabelPropertyService.GetLabels(value.ToString());
                return lpdb.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                var lpdb = Core.Services.LabelPropertyService.GetLabels(value.ToString());
                return lpdb.Name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
