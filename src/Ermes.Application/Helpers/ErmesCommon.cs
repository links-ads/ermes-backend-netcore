﻿using Ermes.Enums;
using Ermes.Net.MimeTypes;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.IO;
using System;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using NetTopologySuite.Geometries;
using Ermes.Core.Helpers;
using Castle.Core.Logging;

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

        public static Geometry GetGeometryFromWKT(string wkt)
        {
            var WKTReader = GetWktReader();
            try
            {
                return WKTReader.Read(wkt);
            }
            catch (Exception)
            {
                return null;
            }
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


        /// <summary>
        /// Proxy function, core function is defined inside Core project
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="thumbnailSize"></param>
        /// <param name="imageQuality"></param>
        /// <returns></returns>
        public static byte[] GetJpegThumbnail(byte[] fileBytes, int thumbnailSize, int imageQuality, ILogger logger)
        {
            return ErmesCoreCommon.CreateThumbnailFromImage(fileBytes, thumbnailSize, imageQuality, logger);
        }
    }
}
