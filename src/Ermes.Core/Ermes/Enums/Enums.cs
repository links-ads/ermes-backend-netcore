using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ermes.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionStatusType
    {
        Off = 10,
        Moving = 20,
        Active = 30
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VisibilityType
    {
        Unknown = 0,
        Public = 10,
        Private = 20,
        Shared = 30
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PersonActionType
    {
        PersonActionSharingPosition = 0,
        PersonActionTracking = 10,
        PersonActionStatus = 20,
        PersonActionActivity = 30
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompetenceAreaType
    {
        Municipality = 10,
        Province = 20,
        Region = 30,
        DrainageBasin = 40,
        AlertArea = 50,
        Other = 100
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MissionStatusType
    {
        Created = 10,
        TakenInCharge = 20,
        Completed = 30,
        Deleted = 90
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralStatus
    {
        Unknown = 0,
        Notified = 10,
        Managed = 20,
        Closed = 30
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryDamage
    {
        Absent = 10,
        Minimum = 20,
        Partial = 30,
        Pervasive = 40,
        Total = 50
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategorySuitability
    {
        Inadeguate = 10,
        Partial = 20,
        Good = 30
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryType
    {
        Range = 0,
        Numeric = 10
    }
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HazardType
    {
        None = 0,
        Flood = 10,
        Fire = 20,
        Landslide = 30,
        Wind = 40,
        Storm = 50,
        Temperature = 60,
        Avalanche = 70,
        Snow = 80,
        Rain = 90,
        Earthquake = 100,
        Subsidence = 110,
        Weather = 120,
        All = 130
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetType
    {
        Others = 0,
        UnsafePeople = 10,
        Roads = 20,
        Schools = 30,
        Hospitals = 40,
        Networks = 50,
        Railroads = 60,
        Bridges = 70,
        Buildings = 80,
        Factories = 90
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationChannelType
    {
        Unknown = 0,
        PushNotification = 10,
        WebApi = 20,
        Bus = 30
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationStatus
    {
        Ok = 0,
        Failed = 10,
        SystemDisabled = 20,
        UserDisabled = 30,
        UserInactive = 40,
        UserUnsubscribed = 50
    }


    [JsonConverter(typeof(StringEnumConverter))]
    public enum SourceDeviceType
    {
        Unknown = 0,
        Chatbot = 10,
        Frontend = 20,
        Smartwatch = 30
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        Communication = 0,
        Mission = 10,
        ReportRequest = 20,
        Other = 30,
        Report = 40,
        PersonActionTracking = 60,
        PersonActionStatus = 70,
        PersonActionActivity = 80,
        Person = 90
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocialModuleTaskType
    {
        none,
        hazard_type,
        information_type,
        named_entity
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SocialModuleLanguageType
    {
        none,
        en,
        it,
        es,
        hr,
        tr,
        fi,
        el,
        fr
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityWriteAction
    {
        Create,
        Update,
        Delete,
        StatusChange
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MediaType
    {
        Image,
        Video,
        Audio,
        File
    }
}
