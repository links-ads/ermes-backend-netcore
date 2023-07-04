namespace Ermes
{
    public class ErmesConsts
    {
        public const string LocalizationSourceName = "Ermes";
        public const string ConnectionStringName = "Default";
        public const string TokenClaim = "Token";
        public const string FusionAuthUserGuidClaim = "FusionAuthUserGuid";
        public const string RolesClaim = "Roles";
        public const string ValidFromClaim = "ValidFrom";
        public const string ValidToClaim = "ValidTo";
        public const string SwaggerAppDocumentName = "app-v1";
        public const string SwaggerBackofficeDocumentName = "backoffice-v1";
        public const string IgnoreApi = "IgnoreApi";
        public const string DefaultProjectName = "FASTER";
        public const string FasterProjectName = "FASTER";
        public const string SafersProjectName = "SAFERS";
        public const string ShelterProjectName = "SHELTER";
        public const string DefaultYear = "2020";
        public const string IMAGE_MIME_TYPE = "image/jpeg";

        public class Thumbnail
        {
            /// <summary>
            /// Thumbnail image export quality
            /// </summary>
            public const int QUALITY = 75;
            /// <summary>
            /// Thumbnail image size
            /// </summary>
            public const int SIZE = 256;
        }
        

        

        public class BusType
        {
            public const string KAFKA = "KAFKA";
            public const string RABBITMQ = "RABBITMQ";
        }


        public class NotificationNames
        {
            public const string Notification = "Notification";
        }

        public struct Environments
        {
            public const string Dev = "dev";
            public const string Test = "Test";
            public const string Prod = "Production";
        }

        public struct UpdateMissionOverdueJob
        {
            public const int startingHour = 0;
            public const string startingCron = "0 0 * * *";
        }

        public class EntityCode
        {
            public const string MapReqeust = "mr";
        }

        public class GamificationActionConsts
        {
            public const string READ_TIP = "ReadTip";
            public const string ANSWER_QUIZ = "AnswerQuiz";
            public const string DO_REPORT = "DoReport";
            public const string FIRST_REPORT = "FirstReport";
            public const string FIRST_LOGIN = "FirstLogin";
            public const string COMPLETE_WIZARD = "CompleteWizard";
            public const string VALIDATE_REPORT = "ValidateReport";
        }

        public struct PurgeStationImagesJob
        {
            public const int startingHour = 1;
            public const string startingCron = "0 1 * * *";
        }
    }
}