using System.Diagnostics;
using Ara3D.Memory;
using Ara3D.Utils;

namespace Ara3D.Graphics
{
    public static class BitmapUtil
    {
        public static void Save(this Bitmap bitmap, FilePath path)
        {
            var writer = path.CreateWrite();
            var dibHeader = new DibHeader(bitmap.Width, bitmap.Height);
            var bmpHeader = new BitmapHeader(dibHeader.biSizeImage);
            writer.WriteValue(bmpHeader);
            Debug.Assert(writer.Position == 14);
            writer.WriteValue(dibHeader);
            Debug.Assert(writer.Position == 54);
            var bytes = bitmap.PixelBuffer.Bytes.AsReadOnlySpan();
            writer.Write(bytes);
            var n = bytes.Length;
            Debug.Assert(n == bitmap.Width * bitmap.Height * 4);
            Debug.Assert(dibHeader.biSizeImage == n);
            Debug.Assert(bmpHeader.Size == 54 + n);
            writer.Close();
        }

        public static Bitmap EvaluateInParallel(this IBitmap source, Bitmap target)
        {
            Parallelizer.ForLoop(source.Width * source.Height, i =>
            {
                var x = i % source.Width;
                var y = i / source.Width;
                var rgb = source.Eval(x, y);
                target.SetPixel(x, y, rgb);
            });

            return target;
        }

        public static Bitmap EvaluateInParallel(this IBitmap source)
        {
            return EvaluateInParallel(source, new Bitmap(source.Width, source.Height));
        }
    }
}