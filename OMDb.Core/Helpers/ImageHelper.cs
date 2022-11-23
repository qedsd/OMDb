using ImageMagick;
using OMDb.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors;

namespace OMDb.Core.Helpers
{
    public static class ImageHelper
    {
        public static void GetImageSize(string path, out double Wpx, out double Hpx)
        {
            try
            {
                MagickImageInfo image = new MagickImageInfo(path);
                int w = image.Width;//宽
                int h = image.Height;//高
                Wpx = image.Density.X;//分辨率
                Hpx = image.Density.Y;//分辨率
                if (image.Density.Units == DensityUnit.PixelsPerCentimeter)//判断分辨率单位
                {
                    Wpx *= 2.54;
                    Hpx *= 2.54;
                }
            }
            catch
            {
                Wpx = 0; Hpx = 0;
            }
        }
        public static ImageInfo GetImageInfo(string path)
        {
            try
            {
                ImageInfo imageInfo = new ImageInfo();
                MagickImageInfo image = new MagickImageInfo(path);
                imageInfo.Width = image.Width;
                imageInfo.Height = image.Height;
                imageInfo.FullPath = path;
                long length = new FileInfo(path).Length;
                imageInfo.Length = length;
                return imageInfo;
            }
            catch
            {
                return null;
            }
        }


        private static async Task<MemoryStream> ToMemoryStreamAsync(Image image)
        {
            MemoryStream stream = new MemoryStream();
            await image.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static async void DrawBannerCoverAsync(List<ImageInfo> covers, ImageInfo bg,string savedPath)
        {
            using (Image image = Image.Load(bg.FullPath))
            {
                List<Image> images = new List<Image>(covers.Count);
                foreach (var cover in covers)
                {
                    images.Add(Image.Load(cover.FullPath));
                }
                int width = (int)(bg.Width * 0.16);
                foreach(Image coverImage in images)
                {
                    coverImage.Mutate(x => x.Resize(width, coverImage.Height * (width/ coverImage.Width)));
                }
                double span = bg.Width * 0.025;
                Point[] points = new Point[images.Count];
                for (int i = images.Count - 1; i >= 0; i--)
                {
                    double startX = bg.Width - span - width;
                    span = bg.Width - startX;
                    double startY = bg.Height / 2 - images[i].Height / 2;
                    points[i] = new Point((int)startX, (int)startY);
                }
                for (int i = images.Count - 1; i >= 0; i--)
                {
                    image.Mutate(c => c.DrawImage(images[i], points[i], 1f));
                    images[i].Dispose();
                }
                await image.SaveAsJpegAsync(savedPath);
            }
        }

        public static async void DrawBannerCoverAsync(List<string> covers, string bg, string savedPath)
        {
            using (Image image = Image.Load(bg))
            {
                List<Image> images = new List<Image>(covers.Count);
                foreach (var cover in covers)
                {
                    images.Add(Image.Load(cover));
                }
                int width = (int)(image.Width * 0.14);
                foreach (Image coverImage in images)
                {
                    coverImage.Mutate(x => x.Resize(width, coverImage.Height * (width / coverImage.Width)));
                }
                double padding = image.Width * 0.022;
                double span = padding;
                Point[] points = new Point[images.Count];
                for (int i = images.Count - 1; i >= 0; i--)
                {
                    double startX = image.Width - span - width;
                    span = image.Width - startX + padding;
                    double startY = image.Height / 2 - images[i].Height / 2;
                    points[i] = new Point((int)startX, (int)startY);
                }
                //image.Mutate(c => c.Lightness(0.7f));
                //image.Mutate(c => c.BoxBlur());
                for (int i = images.Count - 1; i >= 0; i--)
                {
                    image.Mutate(c => c.DrawImage(images[i], points[i], 1f));
                    images[i].Dispose();
                }
                await image.SaveAsJpegAsync(savedPath);
            }
        }

        public static async Task<MemoryStream> DrawWaterfallAsync(List<string> covers, string bg)
        {
            using (Image image = Image.Load(bg))
            {
                List<Image> images = new List<Image>(covers.Count);
                foreach (var cover in covers)
                {
                    images.Add(Image.Load(cover));
                }
                int width = (int)(image.Width * 0.1);
                int height = (int)(width / 0.68);
                foreach (Image coverImage in images)
                {
                    var cutRect = CalPerfectResizeRectangle(coverImage.Width, coverImage.Height, width, height);
                    coverImage.Mutate(x => x.Resize(width, height, KnownResamplers.Bicubic, cutRect, new Rectangle(0, 0, width, height), true));
                }
                int columCount = (int)Math.Ceiling(image.Width / (float)width);
                int rowCount = (int)Math.Ceiling(image.Height / (float)height) + 1;
                int usedCount = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    int baseY = i * height;
                    for (int j = 0; j < columCount; j++)
                    {
                        int startY = baseY - (int)(Math.Pow(-1, j) == 1 ? 0 : 1 * height * 0.5);
                        var currentImage = images[usedCount++ % images.Count];
                        int startX = j * width;
                        if (startY < image.Height)
                        {
                            image.Mutate(c => c.DrawImage(currentImage, new Point(startX, startY), 1f));
                        }
                    }
                }
                foreach(var temp in images)
                {
                    temp.Dispose();
                }
                MemoryStream stream = new MemoryStream();
                await image.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        public static async Task<MemoryStream> DrawWaterfallAsync(List<string> covers, MemoryStream bgStream)
        {
            using (Image image = Image.Load(bgStream.GetBuffer()))
            {
                List<Image> images = new List<Image>(covers.Count);
                foreach (var cover in covers)
                {
                    images.Add(Image.Load(cover));
                }
                int width = (int)(image.Width * 0.1);
                int height = (int)(width / 0.68);
                foreach (Image coverImage in images)
                {
                    var cutRect = CalPerfectResizeRectangle(coverImage.Width, coverImage.Height, width, height);
                    coverImage.Mutate(x => x.Resize(width, height, KnownResamplers.Bicubic, cutRect,new Rectangle(0,0,width,height),true));
                }
                int columCount = (int)Math.Ceiling(image.Width / (float)width);
                int rowCount = (int)Math.Ceiling(image.Height / (float)height) + 1;
                int usedCount = 0;
                for (int i = 0; i < rowCount; i++)
                {
                    int baseY = i * height;
                    for (int j = 0; j < columCount; j++)
                    {
                        int startY = baseY - (int)(Math.Pow(-1, j) == 1 ? 0 : 1 * height * 0.5);
                        var currentImage = images[usedCount++ % images.Count];
                        int startX = j * width;
                        if (startY < image.Height)
                        {
                            image.Mutate(c => c.DrawImage(currentImage, new Point(startX, startY), 1f));
                        }
                    }
                }
                foreach (var temp in images)
                {
                    temp.Dispose();
                }
                MemoryStream stream = new MemoryStream();
                await image.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        /// <summary>
        /// 修改尺寸
        /// </summary>
        /// <param name="path"></param>
        /// <param name="savedPath"></param>
        /// <param name="width">如果为0则依据height等比例缩放</param>
        /// <param name="height">如果为0则依据width等比例缩放</param>
        public static async void ResetSizeAsync(string path,string savedPath, int width, int height)
        {
            using (Image image = Image.Load(path))
            {
                if(width != 0 && height != 0)
                {
                    image.Mutate(x => x.Resize(width, height));
                }
                else if(width != 0)
                {
                    image.Mutate(x => x.Resize(width, image.Height * (width / image.Width)));
                }
                else if(height != 0)
                {
                    image.Mutate(x => x.Resize(image.Width * (height / image.Height), height));
                }
                await image.SaveAsJpegAsync(savedPath);
            }
        }

        /// <summary>
        /// 修改尺寸
        /// </summary>
        /// <param name="path"></param>
        /// <param name="savedPath"></param>
        /// <param name="width">如果为0则依据height等比例缩放</param>
        /// <param name="height">如果为0则依据width等比例缩放</param>
        public static async Task<MemoryStream> ResetSizeAsync(string path, int width, int height)
        {
            using (Image image = Image.Load(path))
            {
                if (width != 0 && height != 0)
                {
                    image.Mutate(x => x.Resize(width, height));
                }
                else if (width != 0)
                {
                    image.Mutate(x => x.Resize(width, image.Height * (width / image.Width)));
                }
                else if (height != 0)
                {
                    image.Mutate(x => x.Resize(image.Width * (height / image.Height), height));
                }
                MemoryStream stream = new MemoryStream();
                await image.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        /// <summary>
        /// 修改尺寸
        /// </summary>
        /// <param name="path"></param>
        /// <param name="savedPath"></param>
        /// <param name="width">如果为0则依据height等比例缩放</param>
        /// <param name="height">如果为0则依据width等比例缩放</param>
        public static MemoryStream ResetSize(string path, int width, int height)
        {
            using (Image image = Image.Load(path))
            {
                if (width != 0 && height != 0)
                {
                    image.Mutate(x => x.Resize(width, height));
                }
                else if (width != 0)
                {
                    image.Mutate(x => x.Resize(width, image.Height * (width / image.Width)));
                }
                else if (height != 0)
                {
                    image.Mutate(x => x.Resize(image.Width * (height / image.Height), height));
                }
                MemoryStream stream = new MemoryStream();
                image.Save(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        /// <summary>
        /// 修改尺寸
        /// </summary>
        /// <param name="inputStream">输入照片流</param>
        /// <param name="width">如果为0则依据height等比例缩放</param>
        /// <param name="height">如果为0则依据width等比例缩放</param>
        public static async Task<MemoryStream> ResetSizeAsync(MemoryStream inputStream, int width, int height)
        {
            using (Image image = Image.Load(inputStream.GetBuffer()))
            {
                if (width != 0 && height != 0)
                {
                    image.Mutate(x => x.Resize(width, height));
                }
                else if (width != 0)
                {
                    image.Mutate(x => x.Resize(width, image.Height * (width / image.Width)));
                }
                else if (height != 0)
                {
                    image.Mutate(x => x.Resize(image.Width * (height / image.Height), height));
                }
                MemoryStream stream = new MemoryStream();
                await image.SaveAsync(stream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder());
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        /// <summary>
        /// 模糊图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> BlurAsync(string path,float sigma = 10)
        {
            using (Image image = Image.Load(path))
            {
                image.Mutate(x => x.GaussianBlur(sigma));
                return await ToMemoryStreamAsync(image);
            }
        }

        /// <summary>
        /// 计算保持原比例裁剪图片时对应的原图区域
        /// </summary>
        /// <param name="srcWidth"></param>
        /// <param name="srcHeight"></param>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <returns></returns>
        private static Rectangle CalPerfectResizeRectangle(int srcWidth,int srcHeight,int targetWidth,int targetHeight)
        {
            Rectangle resultRect;
            //宽高比越大，图片越矮胖
            //宽高比越小，图片越高瘦
            double srcP = srcWidth / (float)srcHeight;
            double targetP = targetWidth / (float)targetHeight;
            if (srcP == targetP)
            {
                resultRect = new Rectangle(0, 0, srcWidth, srcHeight);
            }
            else if (srcP > targetP)
            {
                //原图宽高比大于目标宽高比，说明目标图片要变高瘦
                //保留Y，X要裁剪
                double srcCutWidth = targetWidth * srcHeight / targetHeight;//原图参与缩放的宽
                resultRect = new Rectangle((int)(srcWidth / 2 - srcCutWidth / 2), 0, (int)srcCutWidth, srcHeight);
            }
            else
            {
                //原图宽高比小于目标宽高比，说明目标图片要变矮胖
                //保留X，Y要裁剪
                double srcCutHeight = targetHeight * srcWidth / targetWidth;
                resultRect = new Rectangle(0,(int)(srcHeight / 2 - srcCutHeight / 2),srcWidth, (int)srcCutHeight);
            }
            return resultRect;
        }
    }
}
