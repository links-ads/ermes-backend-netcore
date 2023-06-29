using Castle.Core.Logging;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Ermes.Core.Helpers
{
    public static class ErmesCoreCommon
    {
        public static byte[] CreateThumbnailFromImage(byte[] fileBytes, int thumbnailSize, int imageQuality, ILogger logger)
        {
            logger.Info($"Try to create thumbnail with size: {thumbnailSize}, quality: {imageQuality}");
            using (Image image = Image.FromStream(new MemoryStream(fileBytes)))
            {
                int minSize = Convert.ToInt32(image.Width > image.Height ? image.Height : image.Width);
                logger.Info($"Min size: {minSize}");
                var destRect = new Rectangle(0, 0, thumbnailSize, thumbnailSize);
                var destImage = new Bitmap(thumbnailSize, thumbnailSize);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }

                    EncoderParameters qualityEncodingParameters = new EncoderParameters(1);
                    qualityEncodingParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, imageQuality);
                    logger.Info($"Creating output stream...");
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        var encoder = ImageCodecInfo.GetImageEncoders().Where(i => i.FormatID == ImageFormat.Jpeg.Guid).FirstOrDefault();
                        destImage.Save(outStream, encoder, qualityEncodingParameters);
                        return outStream.ToArray();
                    }
                }

            }
        }
    }
}
