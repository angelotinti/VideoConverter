using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace VideoConverter.XabeFFmpeg
{
    internal class XabeFFmpegConverter
    {
        public async Task<IConversionResult> Convert264(string inputFileName, string outputFileName)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFileName);

            IStream? videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd480);

            var conversion = FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .SetOutput(outputFileName);

            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                Debug.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
            };
            return await conversion.Start();
        }

        public async Task<IConversionResult> ExtractImages(string inputFileName, string outputDirectory, int frameNo)
        {
            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(inputFileName).ConfigureAwait(false);

            IStream? videoStream = mediaInfo.VideoStreams.First()?.SetCodec(VideoCodec.png);

            Func<string, string> buildOutputFileName = (number) =>
            {
                return $"{outputDirectory}\\{Path.GetFileNameWithoutExtension(inputFileName)}_{number}.png";
            };

            var conversion = FFmpeg.Conversions.New()
                .ExtractEveryNthFrame(frameNo, buildOutputFileName)
                .AddStream(videoStream);

            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);
                Debug.WriteLine($"[{args.Duration} / {args.TotalLength}] {percent}%");
            };
            Console.WriteLine($"Starting extraction of images for {inputFileName}");
            var execution = await conversion.Start();
            Console.WriteLine($"Finished extraction of images for {inputFileName}");
            return execution;
        }
    }
}
