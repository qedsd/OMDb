using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace OMDb.WinUI3.Converters
{
    public class IntToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // 将整数值转换为日期时间
            if (value is int intValue)
            {
                // 将整数值映射到时间范围
                double totalValue = 5;
                double mappedDays = (double)intValue / 275 * totalValue;
                double returnValue = mappedDays;
                return returnValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // 将日期时间转换为整数值
            if (value is double rate)
            {

                // 将日期时间映射到整数值
                double totalRate = 5;
                double mappedRate = rate;
                int mappedValue = (int)(mappedRate / totalRate * 275);
                return mappedValue;
            }
            return value;
        }
    }
}
