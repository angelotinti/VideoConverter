namespace VideoConverter.XabeFFmpeg
{
    internal class XabeFFmpegImagesSorter
    {
        public void Sort(string[] images)
        {
            Array.Sort(images, new XabeFFmpegImagesComparer());
        }
    }
}
