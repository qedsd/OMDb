using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace OMDb.Maui.Converters
{
    #region 基础转换器

    /// <summary>
    /// 布尔值转可见度转换器
    /// 将 bool 值转换为 Visibility（显示/隐藏）
    /// 支持反向转换（IsReverse = true 时逻辑反转）
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsReverse { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if ((bool)value)
            {
                return IsReverse ? false : true;
            }
            else
            {
                return IsReverse ? true : false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (value is bool b && b)
            {
                return IsReverse ? false : true;
            }
            else
            {
                return IsReverse ? true : false;
            }
        }
    }

    /// <summary>
    /// 布尔值取反转换器
    /// 将 true 转换为 false，false 转换为 true
    /// 用于需要显示相反逻辑的场景
    /// </summary>
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }
    }

    #endregion

    #region Null 相关转换器

    /// <summary>
    /// Null 值转可见度转换器
    /// null 值转换为 false（隐藏），非 null 转换为 true（显示）
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Null 值转布尔转换器
    /// null 值转换为 false，非 null 转换为 true
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Null 值转 0 转换器
    /// 将 null 转换为 0，用于数值显示
    /// </summary>
    public class NullToZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            if (value is int i) return i;
            if (value is double d) return d;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region 字符串相关转换器

    /// <summary>
    /// 字符串转可见度转换器
    /// 空字符串或 null 转换为 false（隐藏），非空字符串转换为 true（显示）
    /// IsNullOrEmptyToCollapsed = true 时，空字符串也隐藏
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool IsNullOrEmptyToCollapsed { get; set; } = true;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return !IsNullOrEmptyToCollapsed;
            }
            else
            {
                return IsNullOrEmptyToCollapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字符串转可见度转换器（简化版）
    /// 非空字符串返回 true，空字符串或 null 返回 false
    /// </summary>
    public class StrVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region 数据格式化转换器

    /// <summary>
    /// 字节大小格式化转换器
    /// 将字节数转换为人类可读的格式（B、KB、MB、GB、TB）
    /// 例如：1536 转换为 "1.5 KB"
    /// </summary>
    public class ByteSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                int order = 0;
                double size = bytes;
                while (size >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    size /= 1024;
                }
                return $"{size:0.##} {sizes[order]}";
            }
            return value?.ToString() ?? "0 B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region 文件路径转换器

    /// <summary>
    /// 文件路径转目录转换器
    /// 从完整文件路径中提取目录路径
    /// 例如：C:\Videos\movie.mp4 转换为 C:\Videos
    /// </summary>
    public class FileToDirConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return System.IO.Path.GetDirectoryName(value.ToString());
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 文件路径转文件名转换器
    /// 从完整文件路径中提取文件名
    /// WithExtension = true 时包含扩展名，否则不包含
    /// 例如：C:\Videos\movie.mp4 转换为 "movie.mp4" 或 "movie"
    /// </summary>
    public class FileToNameConverter : IValueConverter
    {
        /// <summary>
        /// 是否包含文件扩展名
        /// true = 包含扩展名 (movie.mp4)
        /// false = 不包含扩展名 (movie)
        /// </summary>
        public bool WithExtension { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return WithExtension
                    ? System.IO.Path.GetFileName(value.ToString())
                    : System.IO.Path.GetFileNameWithoutExtension(value.ToString());
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region UI 效果转换器

    /// <summary>
    /// 布尔值转透明度转换器
    /// 用于根据选中状态改变控件的透明度
    /// true = 1.0（完全不透明/高亮）
    /// false = 0.4（半透明/弱化）
    /// </summary>
    public class CheckToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
            {
                return 1.0;
            }
            return 0.4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
