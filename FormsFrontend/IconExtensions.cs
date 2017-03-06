using System.IO;

namespace System.Drawing
{
    internal static class IconExtensions
    {
        internal static Bitmap ToBigBitmap(this Icon icon)
        {
            try
            {
                MemoryStream destStream = new MemoryStream();
                icon.SaveImageToStream(destStream, 0, 0, 32);
                destStream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(destStream);
            }
            catch { return null; }
        }

        internal static void SaveImageToStream(this Icon icon, Stream stream, int width, int height, int depth)
        {
            byte[] srcBuf = null;
            using (MemoryStream readStream = new MemoryStream())
            { icon.Save(readStream); srcBuf = readStream.ToArray(); }
            const int SizeICONDIR = 6;
            const int SizeICONDIRENTRY = 16;
            int iCount = BitConverter.ToInt16(srcBuf, 4);
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                int iWidth = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex];
                int iHeight = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 1];
                int iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                if (iWidth == width && iHeight == height && iBitCount == 32)
                {
                    int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 8);
                    int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 12);
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(srcBuf, iImageOffset, iImageSize);
                    break;
                }
            }
        }

        internal static void SaveImageToFile(this Icon icon, string filename, int width, int height, int depth)
        {
            using (var stream = File.OpenWrite(filename))
            {
                icon.SaveImageToStream(stream, width, height, depth);
            }
        }

    }
}
