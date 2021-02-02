using Abp.ObjectMapping;
using Abp.UI;
using Ermes.Auth.Dto;
using Ermes.CompetenceAreas;
using Ermes.Enums;
using Ermes.Net.MimeTypes;
using Ermes.Persons;
using Ermes.Notifiers;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Ermes.Helpers
{
    public static class ErmesCommon
    {
        public static WKTReader WKTReader;

        public static WKTReader GetWktReader() {
            if (WKTReader == null)
                return new WKTReader()
                {
                    DefaultSRID = AppConsts.Srid
                };
            else
                return WKTReader;
        }

        //https://gist.github.com/ChristianWeyer/eea2cb567932e345cdfa for mapping
        public static string GetMimeTypeFromFileExtension(string fileExtension = "")
        {
            string res;
            switch (fileExtension.ToLower())
            {
                case "jpg":
                case "jpeg":
                    res = MimeTypeNames.ImageJpeg;
                    break;
                case "png":
                    res = MimeTypeNames.ImagePng;
                    break;
                case "mp4":
                case "3gp":
                    res = MimeTypeNames.VideoMp4;
                    break;
                case "mp3":
                case "mpga":
                    res = MimeTypeNames.AudioMpeg;
                    break;
                case "ogg":
                case "oga":
                    res = MimeTypeNames.AudioOgg;
                    break;
                case "adts":
                    res = MimeTypeNames.AudioAdts;
                    break;
                default:
                    res = "";
                    break;
            }

            return res;
        }

        public static MediaType GetMediaTypeFromMimeType(string mimeType)
        {
            if (!Enum.TryParse(mimeType.Split('/').FirstOrDefault(), true, out MediaType mediaType))
                return MediaType.File;
            return mediaType;
        }

        public static MediaType GetMediaTypeFromFilename(string filename)
        {
            string fileExtension = filename.Split(".").LastOrDefault();            ;
            return GetMediaTypeFromMimeType(GetMimeTypeFromFileExtension(fileExtension));
        }

        public static string ValidateFile(IFormFile file, string[] acceptedMimeTypes)
        {
            if (file == null)
                return "InvalidFile";

            if (!ValidateFilename(file.FileName))
                return "InvalidFilename";

            //if (file.Length > 50000000) //50MB
            //    return "FileTooBig";

            if (!acceptedMimeTypes.Contains(file.ContentType))
                return "MimeTypeNotAccepted";

            return null;

        }

        //Example of valid filenames:
        //faster_municipality_2020-02-12
        private static bool ValidateFilename(string fileName)
        {
            if (fileName == null)
                return false;

            fileName = fileName.ToLower();
            //remove file extension and take filename
            var splittedFilenameString = fileName.Split('.')[0].Split('_');
            if (splittedFilenameString.Length != 3)
                return false;

            //TODO: update based on project
            if (!splittedFilenameString[0].Equals("faster"))
                return false;
            if (!DateTime.TryParse(splittedFilenameString[2], out _))
                return false;

            try
            {
                if (!Enum.TryParse(splittedFilenameString[1], true, out CompetenceAreaType result))
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static byte[] GetJpegThumbnail(byte[] fileBytes, int thumbnailSize, int imageQuality)
        {
            using (Image image = Image.FromStream(new MemoryStream(fileBytes)))
            {
                int minSize = Convert.ToInt32(image.Width > image.Height ? image.Height : image.Width);

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
