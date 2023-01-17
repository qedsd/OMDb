﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace OMDb.WinUI3.Converters
{
    public sealed class StringToObject : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                if (value is String)
                {
                    Object fm =value.ToString();
                    return fm;
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
                    Object fm = value.ToString();
                    return fm;
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
