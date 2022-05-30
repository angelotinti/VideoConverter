// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using VideoConverter.Images;
using VideoConverter.MyCam;
using VideoConverter.XabeFFmpeg;

Console.WriteLine("Starting conversion...");

var baseDir = "D:\\MyCam";
var rawDir = Path.Combine(baseDir, "raw");
var imagesDir = Path.Combine(baseDir, "images");
var frameNo = 10;

try
{
    await new MyCamConverter().RawToImages(rawDir, imagesDir, frameNo);
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
}

//16x16 t 5 = 66
//32x32 t 5 = 67
//16x16 t 3 = 97
//32x32 t 3 = 101
//32x32 t 2 = 119

var baseImagesDir = @"D:\MyCam\images";
var imagesDirectories = Directory.GetDirectories(baseImagesDir);
foreach (var imageDirectory in imagesDirectories)
{
    var hoursDirectories = Directory.GetDirectories(imageDirectory);
    foreach (var hourDirectory in hoursDirectories)
    {
        var imagesToCompare = Directory.GetFiles(hourDirectory);
        new XabeFFmpegImagesSorter().Sort(imagesToCompare);
        //pass size and tolerance as parameters
        var result = ImageUtilities.CompareImagesInSequence(imagesToCompare, 32, 2);
        var resultJson = JsonSerializer.Serialize(result);
        File.WriteAllText(Path.Combine(hourDirectory, "result.json"), resultJson);
    }
}

Task.WaitAll();

Console.WriteLine("Conversion finished!!!");