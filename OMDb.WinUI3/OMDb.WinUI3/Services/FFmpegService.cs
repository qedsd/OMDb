using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.WinUI3.Services
{
    internal static class FFmpegService
    {
        static FFmpegService()
        {
            FFmpeg.SetExecutablesPath(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "FFmpeg"));
        }

        public static void Test()
        {
            Services.FFmpegService.AddSubtitleAsync(@"F:\clear.mkv", @"F:\2.mkv", @"F:\1.ass", "fre");
        }
        public static async Task<IMediaInfo> GetMediaInfoAsync(string path)
        {
            return await FFmpeg.GetMediaInfo(path);
        }

        public static async void AddSubtitleAsync(string inputVideo,string outputVideo,string subtitlePath, string lang=null)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.copy);

            IAudioStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.copy);

            IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(subtitlePath);

            ISubtitleStream subtitleStream = subtitleInfo.SubtitleStreams.First().SetLanguage(lang);
            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddStream(videoStream);

            if(mediaInfo.SubtitleStreams.Count() != 0)
            {
                foreach(var subStream in mediaInfo.SubtitleStreams)
                {
                    conversion.AddStream(subStream);
                }
            }

            conversion.AddStream(subtitleStream);
            conversion.SetOutput(outputVideo);
            conversion.OnProgress += Conversion_OnProgress;
            await conversion.Start();
        }

        private static void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
        }
    }
}
