using Abp;
using Abp.Azure.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.UI;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ermes.Resources
{
    public static class ResourceManager
    {

        private const string Reports_Container_Name = "reports";
        private const string Thumbnails_Container_Name = "thumbnails";
        private const string ImportActivities_Container_Name = "import-activities";
        private const string ImportCategories_Container_Name = "import-categories";
        private const string ImportLayers_Container_Name = "import-layers";
        private const string ImportTips_Container_Name = "import-tips";
        private const string ImportQuizzes_Container_Name = "import-quizzes";
        private const string ImportAnswers_Container_Name = "import-answers";
        private const string ImportUsers_Container_Name = "import-users";
        private const string ImportGamificationActions_Container_Name = "import-gamificationactions";
        private static string baseResourcesUrl = string.Empty;
        private static readonly Lazy<IAzureConnectionProvider> _connectionProvider;
        static ResourceManager()
        {
            _connectionProvider = new Lazy<IAzureConnectionProvider>(IocManager.Instance.Resolve<IAzureConnectionProvider>());
        }

        public static string GetBasePath(string entityName)
        {
            return entityName + "/";
        }

        public static string GetFilename(string fullPath)
        {
            var splitted = fullPath.Split('/');
            return splitted.LastOrDefault();
        }

        private static string BuildResourceUrl(string context, int identifier, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;

            Uri uri = new Uri(GetBaseResourcesUrl());
            IList<string> parts = new List<string>
            {
                BuildResourcePath(context, identifier, fileName)
            };

            uri = new Uri(parts.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
            return uri.AbsoluteUri;
        }

        private static string BuildRelativeResourceUrl(int identifier, string fileName)
        {
            return (FormatIdentifier(identifier) + "/" + fileName).Trim();
        }

        public static class Reports
        {
            public static string ContainerName { get { return Reports_Container_Name; } }
            public static string GetMediaPath(int reportId, string fileName)
            {
                return BuildResourceUrl(Reports_Container_Name, reportId, fileName);
            }

            public static string GetRelativeMediaPath(int reportId, string fileName)
            {
                return BuildRelativeResourceUrl(reportId, fileName);
            }
        }

        public static class Thumbnails
        {
            public static string ContainerName { get { return Thumbnails_Container_Name; } }
            public static string GetJpegThumbnailFilename(string fileName)
            {
                //TODO: function to be checked
                //return fileName.Split('.').FirstOrDefault() + ".jpg";
                return fileName;
            }
            public static string GetMediaPath(int reportId, string fileName)
            {
                return BuildResourceUrl(Thumbnails_Container_Name, reportId, fileName);
            }

            public static string GetRelativeMediaPath(int reportId, string fileName)
            {
                return BuildRelativeResourceUrl(reportId, fileName);
            }
        }

        public interface IImportResourceContainer
        {
            public string ContainerName { get; }
            public string FileNameBase { get; }
            public string GetRelativeMediaPath(bool success, string fileName)
            {
                return ((success ? "successes" : "failures") + "/" + fileName).Trim();
            }
        }

        public class ImportActivitiesResourceContainer: IImportResourceContainer
        {
            public string ContainerName { get { return ImportActivities_Container_Name; } }
            public string FileNameBase { get { return "activities-";  } }

        }

        public class ImportCategoriesResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportCategories_Container_Name; } }
            public string FileNameBase { get { return "categories-"; } }

        }

        public class ImportLayersResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportLayers_Container_Name; } }
            public string FileNameBase { get { return "layers-"; } }

        }

        public class ImportUsersResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportUsers_Container_Name; } }
            public string FileNameBase { get { return "users-"; } }
        }

        public class ImportTipsResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportTips_Container_Name; } }
            public string FileNameBase { get { return "tips-"; } }
        }

        public class ImportQuizzesResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportQuizzes_Container_Name; } }
            public string FileNameBase { get { return "quizzes-"; } }
        }

        public class ImportAnswersResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportAnswers_Container_Name; } }
            public string FileNameBase { get { return "answers-"; } }
        }

        public class ImportGamificationActionsResourceContainer : IImportResourceContainer
        {
            public string ContainerName { get { return ImportGamificationActions_Container_Name; } }
            public string FileNameBase { get { return "gamificationactions-"; } }
        }

        private static string GetBaseResourcesUrl()
        {

            if (!string.IsNullOrWhiteSpace(baseResourcesUrl)) 
                return baseResourcesUrl;

            if (!_connectionProvider.IsValueCreated)
                throw new UserFriendlyException("InvalidAzureSettings");

            baseResourcesUrl = _connectionProvider.Value.GetStorageBasePath();
            return baseResourcesUrl;
        }

        public static string FormatIdentifier(int identifier)
        {
            return identifier.ToString().PadLeft(6, '0');
        }

        private static string BuildResourcePath(string context, int identifier, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;

            IList<string> parts = new List<string>();

            parts.Add(context);
            parts.Add(FormatIdentifier(identifier));
            parts.Add(fileName);

            return string.Join("/", parts).Trim();
        }
    }
}
