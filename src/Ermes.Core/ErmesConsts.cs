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
    }
}