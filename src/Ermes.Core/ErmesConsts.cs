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


        public class NotificationNames
        {
            public const string Notification = "Notification";
        }

        public struct Environments
        {
            public const string Dev = "Development";
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
        }
    }
}