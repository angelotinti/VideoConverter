namespace VideoConverter.Images
{
    internal class ImagesComparisonResult
    {
        public string BaseDir { get; set; }
        public IList<DivergentImages> MyProperty { get; set; }

        public ImagesComparisonResult(string baseDir)
        {
            BaseDir = baseDir;
            MyProperty = new List<DivergentImages>();
        }
    }
}
