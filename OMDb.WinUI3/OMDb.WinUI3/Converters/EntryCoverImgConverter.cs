using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OMDb.WinUI3.Converters
{
    public sealed class EntryCoverImgConverter : IValueConverter
    {
        public enum EntryCoverImgConverterMode
        {
            String,
            Bitmap,
        }
        public EntryCoverImgConverterMode CoverMode { get; set; } = EntryCoverImgConverterMode.String;
        public int Width { get; set; }
        public int Height { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var entry = value as Core.Models.Entry;
            if (entry != null)
            {
                var storage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
                if(storage != null)
                {
                    var path =  string.IsNullOrEmpty(storage.StoragePath)?null: System.IO.Path.Combine(System.IO.Path.GetDirectoryName(storage.StoragePath), entry.Path, entry.CoverImg);
                    if(CoverMode == EntryCoverImgConverterMode.String)
                    {
                        return path;
                    }
                    else if(CoverMode == EntryCoverImgConverterMode.Bitmap)
                    {
                        var stream = Core.Helpers.ImageHelper.ResetSize(path, Width, Height);
                        return Helpers.ImgHelper.CreateBitmapImage(stream);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        public static string Convert(Core.Models.Entry entry)
        {
            var storage = Services.ConfigService.EnrtyStorages.FirstOrDefault(p => p.StorageName == entry.DbId);
            if (storage != null)
            {
                return string.IsNullOrEmpty(storage.StoragePath) ? null : System.IO.Path.Combine(System.IO.Path.GetDirectoryName(storage.StoragePath), entry.Path, entry.CoverImg);
            }
            else
            {
                return null;
            }
        }
    }
}
