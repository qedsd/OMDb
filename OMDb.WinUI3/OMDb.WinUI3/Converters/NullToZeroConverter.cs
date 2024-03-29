﻿using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Converters
{
    public sealed class NullToBooleanConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public bool EnforceNonWhiteSpaceString { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value?.GetType() == typeof(string))
            {
                if (IsInverted)
                {
                    if (EnforceNonWhiteSpaceString)
                    {
                        return !string.IsNullOrWhiteSpace((string)value);
                    }
                    return !string.IsNullOrEmpty((string)value);
                }

                if (EnforceNonWhiteSpaceString)
                {
                    return !string.IsNullOrWhiteSpace((string)value);
                }
                return string.IsNullOrEmpty((string)value);
            }

            if (IsInverted)
            {
                return value != null;
            }
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
