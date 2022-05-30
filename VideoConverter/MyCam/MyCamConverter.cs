using Microsoft.Extensions.Logging;
using VideoConverter.XabeFFmpeg;

namespace VideoConverter.MyCam
{
    internal class MyCamConverter
    {
        public async Task RawToImages(string rawDir, string imagesDir, int frameNo)
        {
            Console.WriteLine($"Raw to images (rawDir: {rawDir}, imagesDir: {imagesDir}, frameNo: {frameNo})");
            var rawDirectories = Directory.EnumerateDirectories(rawDir);

            await Parallel.ForEachAsync(rawDirectories, async (rawDirectory, y) =>
            {
                await HandleSingleRawDir(imagesDir, frameNo, rawDirectory);
            });
        }

        private static async Task HandleSingleRawDir(string imagesDir, int frameNo, string rawDirectory)
        {
            Console.WriteLine($"Raw directory: {rawDirectory}");
            var rawDirNameInt = int.Parse(Path.GetFileNameWithoutExtension(rawDirectory));
            var newDirName = new DateTime(1970, 1, 1).AddSeconds(rawDirNameInt).ToString("yyyy-MM-dd");
            var newDirPath = Path.Combine(imagesDir, newDirName);

            //if(Directory.Exists(newDirPath) && 
            //    Directory.EnumerateFiles(newDirPath).Count() == Directory.EnumerateFiles(rawDirectory).Count())
            //{
            //    continue;
            //}

            Directory.CreateDirectory(newDirPath);

            var rawFiles = Directory.EnumerateFiles(rawDirectory);

            foreach (var rawFile in rawFiles)
            {
                var rawFileNameSplitted = Path.GetFileNameWithoutExtension(rawFile).Split("_");
                var startDateString = new DateTime(1970, 1, 1).AddSeconds(int.Parse(rawFileNameSplitted[0])).ToString("HHmmss");
                var rawFileEndInt = int.Parse(rawFileNameSplitted[1]);
                string endDateString = "no_end";
                if (rawFileEndInt > 0)
                {
                    endDateString = new DateTime(1970, 1, 1).AddSeconds(rawFileEndInt).ToString("HHmmss");
                }
                var imagesDirectoryName = Path.Combine(newDirPath, $"{startDateString}_{endDateString}");

                //skip if already processed
                if (Directory.Exists($"{imagesDirectoryName}_"))
                {
                    Console.WriteLine($"Skipping {imagesDirectoryName}_");
                    continue;
                }

                Directory.CreateDirectory(imagesDirectoryName);

                //create interface for image extractor and receive on ctor
                await new XabeFFmpegConverter().ExtractImages(rawFile, imagesDirectoryName, frameNo);

                //change directory name to indicate it's been already processed
                Directory.Move(imagesDirectoryName, $"{imagesDirectoryName}_");
            }
        }

    }
}
