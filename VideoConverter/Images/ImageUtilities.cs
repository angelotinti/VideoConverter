using System.Drawing;
using System.Drawing.Drawing2D;

namespace VideoConverter.Images
{
    internal class ImageUtilities
    {
        public static Image ResizeImage(Image imgToResize, int imagesSize)
        {
            var size = new Size(imagesSize, imagesSize);
            //Get the image current width
            int sourceWidth = imgToResize.Width;
            //Get the image current height
            int sourceHeight = imgToResize.Height;
            //Calulate  width with new desired size
            float nPercentW = size.Width / (float)sourceWidth;
            //Calculate height with new desired size
            float nPercentH = size.Height / (float)sourceHeight;
            float nPercent;
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        public static void ToBlackWhite(Image image)
        {
            using (Graphics gr = Graphics.FromImage(image)) //SourceImage is a Bitmap object
            {
                var gray_matrix = new float[][] {
                new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                new float[] { 0,      0,      0,      1, 0 },
                new float[] { 0,      0,      0,      0, 1 }
            };

                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
                ia.SetThreshold(0.8f); //Change this threshold as needed
                var rc = new Rectangle(0, 0, image.Width, image.Height);
                gr.DrawImage(image, rc, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
            }
        }

        public static List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            //create new image with bmpSource Widht x bmpSource Height pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(bmpSource.Width, bmpSource.Height));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        public static bool CompareImages(Image firstImage, Image secondImage, float tolerance)
        {
            List<bool> iHash1 = GetHash(new Bitmap(firstImage));
            List<bool> iHash2 = GetHash(new Bitmap(secondImage));

            //determine the number of equal pixel (x of 256 for size 16)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            var totalSize = firstImage.Width * firstImage.Height;
            return equalElements > totalSize * (100 - tolerance) / 100;
        }

        public static IList<DivergentImages> CompareImagesInSequence(string[] imagesToCompare, int imagesSize, float tolerance)
        {
            var result = new List<DivergentImages>();
            if (imagesToCompare.Length > 1)
            {
                var contextImage = Image.FromFile(imagesToCompare[0]);
                var contextImageName = imagesToCompare[0];
                for (int i = 1; i < imagesToCompare.Length; i++)
                {
                    var imageToCompare = Image.FromFile(imagesToCompare[i]);
                    var imageToCompareName = imagesToCompare[i];

                    var contextImageResized = ImageUtilities.ResizeImage(contextImage, imagesSize);
                    var compareImageResized = ImageUtilities.ResizeImage(imageToCompare, imagesSize);

                    ImageUtilities.ToBlackWhite(contextImageResized);
                    ImageUtilities.ToBlackWhite(compareImageResized);

                    var areEqual = ImageUtilities.CompareImages(contextImageResized, compareImageResized, tolerance);

                    if (!areEqual)
                    {
                        result.Add(new DivergentImages
                        {
                            BaseImage = contextImageName,
                            DivergentImage = imageToCompareName
                        });
                    }

                    contextImage = imageToCompare;
                    contextImageName = imageToCompareName;
                }
            }
            return result;
        }
    }
}
