using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using OMDb.Core.Models.EntryModels;
using OMDb.WinUI3.Services;
using System;

namespace OMDb.WinUI3.Converters
{
    public class IntToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // 将整数值转换为日期时间
            if (value is int intValue)
            {
                var minDate = EntryService.MaxMinDateModel.MinDate;
                var maxDate = EntryService.MaxMinDateModel.MaxDate;

                // 计算时间范围的起始日期和结束日期
                DateTimeOffset startDate = minDate;
                DateTimeOffset endDate = maxDate;

                // 将整数值映射到时间范围
                double totalDays = (endDate - startDate).TotalDays;
                double mappedDays = (double)intValue / 275 * totalDays;
                DateTimeOffset mappedDate = startDate.AddDays(mappedDays);

                return mappedDate;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // 将日期时间转换为整数值
            if (value is DateTimeOffset date)
            {
                var minDate = EntryService.MaxMinDateModel.MinDate;
                var maxDate = EntryService.MaxMinDateModel.MaxDate;

                // 计算时间范围的起始日期和结束日期
                DateTimeOffset startDate = minDate;
                DateTimeOffset endDate = maxDate;

                // 将日期时间映射到整数值
                double totalDays = (endDate - startDate).TotalDays;
                double mappedDays = (date - startDate).TotalDays;
                int mappedValue = (int)(mappedDays / totalDays * 275);

                return mappedValue;
            }
            return value;
        }
    }
}
