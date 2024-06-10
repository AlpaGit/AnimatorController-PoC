using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AnimatorController.Models;

#pragma warning disable CA1416

namespace AnimatorController.Readers
{
    public class AnimationAtlas
    {
        public string Path { get; }
        public IList<Graphic> GraphicDatas { get; }
        
        public AnimationAtlas(string path, IList<Graphic> graphics)
        {
            Path = path;
            GraphicDatas = graphics;
        }

        public void Export()
        {
            using var atlasImage = new Bitmap(Path);
            var atlasWidth = atlasImage.Width;
            var atlasHeight = atlasImage.Height;

            var index = 0;
            foreach (var graphic in GraphicDatas)
            {
                var uvPixels = new PointF[graphic.Uvs.Count];
                for (var i = 0; i < graphic.Uvs.Count; i++)
                {
                    uvPixels[i] = new PointF(graphic.Uvs[i].X * atlasWidth, (1 - graphic.Uvs[i].Y) * atlasHeight);
                }

                // Create a mask for the shape using triangles
                var mask = CreateMask(atlasWidth, atlasHeight, uvPixels, graphic.Triangles);

                // Use the mask to create the cropped image
                var croppedImage = new Bitmap(atlasWidth, atlasHeight, PixelFormat.Format32bppArgb);
                ApplyMaskToImage(atlasImage, mask, croppedImage);

                // Calculate the bounding box of the shape
                var boundingBox = GetBoundingBox(mask);

                // Crop the image to the bounding box
                var finalImage = croppedImage.Clone(boundingBox, croppedImage.PixelFormat);

                // Save the cropped image
                var outputPath = $"atlas-{index++}.png";
                finalImage.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Saved cropped image to {outputPath}");
            }
        }
        
        private static Bitmap CreateMask(int width, int height, PointF[] uvPixels, IList<int> triangles)
        {
            var mask = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using var g = Graphics.FromImage(mask);
            g.Clear(Color.Black);

            using var brush = new SolidBrush(Color.White);
            for (var i = 0; i < triangles.Count; i += 3)
            {
                var trianglePoints = new[]
                {
                    uvPixels[triangles[i]],
                    uvPixels[triangles[i + 1]],
                    uvPixels[triangles[i + 2]]
                };
                g.FillPolygon(brush, trianglePoints);
            }

            return mask;
        }


        private static void ApplyMaskToImage(Bitmap source, Bitmap mask, Bitmap destination)
        {
            var sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var destData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            var bytes = sourceData.Stride * sourceData.Height;
            var sourceBuffer = new byte[bytes];
            var maskBuffer = new byte[bytes];
            var destBuffer = new byte[bytes];

            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, bytes);
            Marshal.Copy(maskData.Scan0, maskBuffer, 0, bytes);

            for (var i = 0; i < bytes; i += 4)
            {
                if (maskBuffer[i] > 0)
                {
                    destBuffer[i] = sourceBuffer[i];
                    destBuffer[i + 1] = sourceBuffer[i + 1];
                    destBuffer[i + 2] = sourceBuffer[i + 2];
                    destBuffer[i + 3] = sourceBuffer[i + 3];
                }
                else
                {
                    destBuffer[i + 3] = 0; // Set alpha to 0 for transparency
                }
            }

            Marshal.Copy(destBuffer, 0, destData.Scan0, bytes);

            source.UnlockBits(sourceData);
            mask.UnlockBits(maskData);
            destination.UnlockBits(destData);
        }


        private static Rectangle GetBoundingBox(Bitmap mask)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            var maskData = mask.LockBits(new Rectangle(0, 0, mask.Width, mask.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var bytes = maskData.Stride * maskData.Height;
            var maskBuffer = new byte[bytes];
            Marshal.Copy(maskData.Scan0, maskBuffer, 0, bytes);

            for (var y = 0; y < mask.Height; y++)
            {
                for (var x = 0; x < mask.Width; x++)
                {
                    var index = y * maskData.Stride + x * 4;
                    if (maskBuffer[index] <= 0)
                        continue;

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }

            mask.UnlockBits(maskData);
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }
    }
}
