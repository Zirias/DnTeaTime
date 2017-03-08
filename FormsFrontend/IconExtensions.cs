using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Drawing
{
    internal static class IconExtensions
    {
        internal static Image ToBitmap(this Icon icon, int width, int height, int depth = 32)
        {
            try
            {
                MemoryStream destStream = new MemoryStream();
                icon.SaveImageToStream(destStream, width, height, depth);
                destStream.Seek(0, SeekOrigin.Begin);
                if (depth == 32)
                {
                    var bmp = (byte[])destStream.GetBuffer().Clone();
                    var magic = Encoding.ASCII.GetString(bmp, 1, 3);
                    if (magic == "PNG") return new Bitmap(destStream);
                    var handle = GCHandle.Alloc(bmp, GCHandleType.Pinned);
                    try
                    {
                        var pixels = (IntPtr)((int)(handle.AddrOfPinnedObject()) + 54);
                        var result = new Bitmap(width, height, 4 * width, PixelFormat.Format32bppArgb, pixels);
                        result.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        return result;
                    }
                    finally
                    {
                        handle.Free();
                        destStream.Dispose();
                    }
                }
                else
                {
                    return new Bitmap(destStream);
                }
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
                var iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                if (iWidth == width && iHeight == height && iBitCount == depth)
                {
                    int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 8);
                    int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 12);
                    var magic = Encoding.ASCII.GetString(srcBuf, iImageOffset + 1, 3);
                    var writer = new BinaryWriter(stream);
                    if (magic != "PNG")
                    {
                        // BMP format, restore BITMAPFILEHEADER
                        const int BI_BITFIELDS = 3;
                        const int BI_ALPHABITFIELDS = 6;
                        int numColors = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 2];
                        int headerSize = BitConverter.ToInt32(srcBuf, iImageOffset);
                        int compressionMethod = BitConverter.ToInt32(srcBuf, iImageOffset + 30);
                        int pixelDataOffset = 14 + headerSize + numColors * ((3 * iBitCount) / 8);
                        if (compressionMethod == BI_BITFIELDS) pixelDataOffset += 12;
                        if (compressionMethod == BI_ALPHABITFIELDS) pixelDataOffset += 16;
                        writer.Write(Encoding.ASCII.GetBytes("BM"));
                        writer.Write(iImageSize + 14);
                        writer.Write(0);
                        writer.Write(pixelDataOffset);

                        // Fix DIB header info
                        if (headerSize == 12)
                        {
                            // BITMAPCOREHEADER
                            var w16 = Convert.ToInt16(iWidth);
                            var h16 = Convert.ToInt16(iHeight);
                            BitConverter.GetBytes(w16).CopyTo(srcBuf, iImageOffset + 4);
                            BitConverter.GetBytes(h16).CopyTo(srcBuf, iImageOffset + 6);
                            BitConverter.GetBytes(iBitCount).CopyTo(srcBuf, iImageOffset + 10);
                        }
                        else
                        {
                            // BITMAPINFOHEADER
                            BitConverter.GetBytes(iWidth).CopyTo(srcBuf, iImageOffset + 4);
                            BitConverter.GetBytes(iHeight).CopyTo(srcBuf, iImageOffset + 8);
                            BitConverter.GetBytes(iBitCount).CopyTo(srcBuf, iImageOffset + 14);
                        }
                    }
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
