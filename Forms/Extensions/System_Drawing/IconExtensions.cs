using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace System.Drawing
{
    public static class IconExtensions
    {
        public static Bitmap ToBitmap(this Icon icon, int maxWidth, int maxHeight = -1,
            int maxDepth = 32, int minWidth = 1, int minHeight = -1, int minDepth = 1)
        {
            int matchingIndex = -1;
            int matchingWidth = 0;
            int matchingHeight = 0;
            int matchingDepth = 0;

            if (maxHeight < 0) maxHeight = maxWidth;
            if (minHeight < 0) minHeight = minWidth;

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
                if (iWidth == 0) iWidth = 256;
                if (iHeight == 0) iHeight = 256;
                var iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                if (iWidth < minWidth || iWidth > maxWidth ||
                    iHeight < minHeight || iHeight > maxHeight ||
                    iBitCount < minDepth || iBitCount > maxDepth)
                    continue;
                if (iWidth >= matchingWidth && iHeight >= matchingHeight && iBitCount >= matchingDepth)
                {
                    matchingIndex = iIndex;
                    matchingWidth = iWidth;
                    matchingHeight = iHeight;
                    matchingDepth = iBitCount;
                    if (matchingWidth == maxWidth && matchingHeight == maxHeight && matchingDepth == maxDepth) break;
                }
            }
            if (matchingIndex < 0) return null;
            int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * matchingIndex + 8);
            int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * matchingIndex + 12);
            var magic = Encoding.ASCII.GetString(srcBuf, iImageOffset + 1, 3);
            if (magic == "PNG")
            {
                // Bitmap class reads any PNG directly from stream
                var stream = new MemoryStream();
                var writer = new BinaryWriter(stream);
                writer.Write(srcBuf, iImageOffset, iImageSize);
                stream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(stream);
            }
            else
            {
                // BMP format, determine metadata
                const int BI_BITFIELDS = 3;
                const int BI_ALPHABITFIELDS = 6;
                int numColors = srcBuf[SizeICONDIR + SizeICONDIRENTRY * matchingIndex + 2];
                if (numColors == 0 && matchingDepth <= 8) numColors = 1 << matchingDepth;
                int headerSize = BitConverter.ToInt32(srcBuf, iImageOffset);
                int compressionMethod = BitConverter.ToInt32(srcBuf, iImageOffset + 30);
                int pixelDataOffset = headerSize + numColors * (headerSize == 12 ? 3 : 4);
                if (compressionMethod == BI_BITFIELDS) pixelDataOffset += 12;
                if (compressionMethod == BI_ALPHABITFIELDS) pixelDataOffset += 16;

                if (matchingDepth == 32)
                {
                    // BMP with 32 bits not directly supported, load pixels ourselves
                    unsafe
                    {
                        fixed (byte* srcBufPtr = srcBuf)
                        {
                            var pixels = srcBufPtr + iImageOffset + pixelDataOffset;
                            var result = new Bitmap(matchingWidth, matchingHeight, PixelFormat.Format32bppArgb);
                            var resultBits = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                            byte* dest = (byte*)resultBits.Scan0;
                            for (int y = matchingHeight - 1; y >= 0; --y)
                            {
                                byte* src = pixels + y * resultBits.Stride;
                                for (int i = resultBits.Stride; i > 0; --i) *dest++ = *src++;
                            }
                            result.UnlockBits(resultBits);
                            return result;
                        }
                    }
                }
                else
                {
                    bool upsideDown;

                    // Add size of BITMAPFILEHEADER
                    pixelDataOffset += 14;

                    // Fix DIB header info
                    if (headerSize == 12)
                    {
                        // BITMAPCOREHEADER
                        upsideDown = BitConverter.ToInt16(srcBuf, iImageOffset + 6) >= 0;
                        var w16 = Convert.ToInt16(matchingWidth);
                        var h16 = Convert.ToInt16(matchingHeight);
                        BitConverter.GetBytes(w16).CopyTo(srcBuf, iImageOffset + 4);
                        BitConverter.GetBytes(h16).CopyTo(srcBuf, iImageOffset + 6);
                        BitConverter.GetBytes(matchingDepth).CopyTo(srcBuf, iImageOffset + 10);
                    }
                    else
                    {
                        // BITMAPINFOHEADER
                        upsideDown = BitConverter.ToInt32(srcBuf, iImageOffset + 8) >= 0;
                        BitConverter.GetBytes(matchingWidth).CopyTo(srcBuf, iImageOffset + 4);
                        BitConverter.GetBytes(matchingHeight).CopyTo(srcBuf, iImageOffset + 8);
                        BitConverter.GetBytes(matchingDepth).CopyTo(srcBuf, iImageOffset + 14);
                    }

                    Bitmap result;
                    using (var stream = new MemoryStream())
                    {
                        var writer = new BinaryWriter(stream);

                        // Write BITMAPFILEHEADER
                        writer.Write(Encoding.ASCII.GetBytes("BM"));
                        writer.Write(iImageSize + 14);
                        writer.Write(0);
                        writer.Write(pixelDataOffset);

                        writer.Write(srcBuf, iImageOffset, iImageSize);
                        stream.Seek(0, SeekOrigin.Begin);
                        var tmp = new Bitmap(stream);
                        result = new Bitmap(tmp.Width, tmp.Height, PixelFormat.Format32bppArgb);
                        using (var g = Graphics.FromImage(result))
                        {
                            g.DrawImage(tmp, 0, 0);
                        }
                    }
                    int srcStride = matchingWidth * matchingDepth / 8;
                    int srcStrideModulus = srcStride % 4;
                    if (srcStrideModulus != 0) srcStride += 4 - srcStrideModulus;
                    var resultBits = result.LockBits(new Rectangle(0, 0, result.Width, result.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    int extraBits = 32 - matchingWidth % 32;
                    if (extraBits == 32) extraBits = 0;
                    int skipBytes = extraBits / 8;
                    if (extraBits % 8 != 0) ++skipBytes;
                    unsafe
                    {
                        fixed (byte* srcBufPtr = srcBuf)
                        {
                            byte* andMask = srcBufPtr + iImageOffset + pixelDataOffset - 14 + matchingHeight * srcStride;
                            int bitmask = 1 << 7;
                            for (int y = 0; y < matchingHeight; ++y)
                            {
                                byte* outLine = (byte*)resultBits.Scan0;
                                if (upsideDown) outLine += (matchingHeight - 1 - y) * resultBits.Stride;
                                else outLine += y * resultBits.Stride;

                                for (byte* outAlpha = outLine + 3; outAlpha < outLine + 4 * matchingWidth; outAlpha += 4)
                                {
                                    if ((*andMask & bitmask) > 0) *outAlpha = 0;
                                    bitmask >>= 1;
                                    if (bitmask == 0)
                                    {
                                        bitmask = 1 << 7;
                                        ++andMask;
                                    }
                                }
                                if (extraBits > 0)
                                {
                                    bitmask = 1 << 7;
                                    andMask += skipBytes;
                                }
                            }
                        }
                    }
                    result.UnlockBits(resultBits);
                    return result;
                }
            }
        }
    }
}
