using OMDb.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace OMDb.Core.Helpers
{
    public static class FFmpegHelper
    {
        //TODO:目前的音频、视频流都假设为只有一个

        internal static void SetExecutablesPath(string path)
        {
            FFmpeg.SetExecutablesPath(path);
        }
        public static async Task<Models.MediaInfo> GetMediaInfoAsync(string path)
        {
            var info = await FFmpeg.GetMediaInfo(path);
            return new Models.MediaInfo(info);
        }

        public static async Task AddSubtitleAsync(string inputVideo, string outputVideo, SubtitleInfo newsubtitle)
        {
            await AddSubtitleAsync(inputVideo,outputVideo, new List<SubtitleInfo>() { newsubtitle });
        }

        public static async Task AddSubtitleAsync(string inputVideo, string outputVideo, List<SubtitleInfo> newsubtitles)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.copy);

            IAudioStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(AudioCodec.copy);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddStream(videoStream);

            if (mediaInfo.SubtitleStreams.Count() != 0)
            {
                foreach (var subStream in mediaInfo.SubtitleStreams)
                {
                    conversion.AddStream(subStream);
                }
            }

            foreach (var newSubtitle in newsubtitles)
            {
                IMediaInfo subtitleInfo = await FFmpeg.GetMediaInfo(newSubtitle.Path);
                var newSubStream = subtitleInfo.SubtitleStreams.First();
                newSubStream.SetCodec(newSubtitle.Codec);
                newSubStream.SetLanguage(newSubtitle.Language);
                //TODO:title?
                conversion.AddStream(newSubStream);
            }

            conversion.SetOutput(outputVideo);
            conversion.OnProgress += Conversion_OnProgress;
            await conversion.Start();
        }

        private static void Conversion_OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs args)
        {
            var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
            Console.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
        }


        /// <summary>
        /// 视频、音频编码转换
        /// </summary>
        /// <param name="inputVideo"></param>
        /// <param name="outputVideo"></param>
        /// <param name="vcodec"></param>
        /// <param name="acodec"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public static async Task ConverAVCodecAsync(string inputVideo, string outputVideo, string vcodec, string acodec, Xabe.FFmpeg.Events.ConversionProgressEventHandler progressCallback = null)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputVideo);
            IVideoStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(vcodec);

            IAudioStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()
                ?.SetCodec(acodec);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(audioStream)
                .AddStream(videoStream);

            if (mediaInfo.SubtitleStreams.Count() != 0)
            {
                foreach (var subStream in mediaInfo.SubtitleStreams)
                {
                    conversion.AddStream(subStream);
                }
            }

            conversion.SetOutput(outputVideo);
            if(progressCallback != null)
            {
                conversion.OnProgress += progressCallback;
            }
            await conversion.Start();
        }
    }
}
