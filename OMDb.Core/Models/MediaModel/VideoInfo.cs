using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Models
{
    public class VideoInfo
    {
        public TimeSpan Duration { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double Framerate { get; set; }

        public string Ratio { get; set; }

        public long Bitrate { get; set; }

        public int? Default { get; set; }

        public int? Forced { get; set; }

        /// <summary>
        /// 像素格式
        /// </summary>
        public string PixelFormat { get; set; }

        public int? Rotation { get; set; }

        public string Codec { get; set; }

        public VideoInfo(IVideoStream videoStream)
        {
            Duration = videoStream.Duration;
            Width = videoStream.Width;
            Height = videoStream.Height;
            Framerate = videoStream.Framerate;
            Ratio = videoStream.Ratio;
            Bitrate = videoStream.Bitrate;
            Default = videoStream.Default;
            Forced = videoStream.Forced;
            PixelFormat = videoStream.PixelFormat;
            Rotation = videoStream.Rotation;
            Codec = videoStream.Codec;
        }
    }
}
