namespace VideoConverter.XabeFFmpeg
{
    internal class XabeFFmpegImagesComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            var xInt = int.Parse(Path.GetFileNameWithoutExtension(x).Split("__")[1]);
            var yInt = int.Parse(Path.GetFileNameWithoutExtension(y).Split("__")[1]);
            return xInt - yInt;
        }
    }
}
