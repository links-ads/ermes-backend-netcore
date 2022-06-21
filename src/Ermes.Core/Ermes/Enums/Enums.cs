using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ermes.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionStatusType
    {
        Off = 10,
        Ready = 15,
        Moving = 20,
        Active = 30
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

    //OLD LIST
    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum HazardType
    //{
    //    None = 0,
    //    Flood = 10,
    //    Fire = 20,
    //    Landslide = 30,
    //    Wind = 40,
    //    Storm = 50,
    //    Temperature = 60,
    //    Avalanche = 70,
    //    Snow = 80,
    //    Rain = 90,
    //    Earthquake = 100,
    //    Subsidence = 110,
    //    Weather = 120,
    //    All = 130
    //}

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HazardType
    {
        None = 0,
        Avalanche = 10,
        Earthquake = 20,
        Fire = 30,
        Flood = 40,
        Landslide = 50,
        Storm = 60,
        Weather = 70,
        Subsidence = 80
    }
    //old list
    //[JsonConverter(typeof(StringEnumConverter))]
    //public enum TargetType
    //{
    //    Others = 0,
    //    UnsafePeople = 10,
    //    Roads = 20,
    //    Schools = 30,
    //    Hospitals = 40,
    //    Networks = 50,
    //    Railroads = 60,
    //    Bridges = 70,
    //    Buildings = 80,
    //    Factories = 90
    //}

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetType
    {
        None = 0,
        Buildings = 10,
        CulturalHeritage = 20,
        Factories = 30,
        Hospitals = 40,
        Infrastructures = 50,
        Networks = 60,
        Schools = 70,
        Other = 80
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
        Smartwatch = 30,
        AugmentedReality = 40
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityType
    {
        Communication = 0,
        MapRequest = 5,
        Mission = 10,
        ReportRequest = 20,
        Other = 30,
        Report = 40,
        PersonActionTracking = 60,
        PersonActionStatus = 70,
        PersonActionActivity = 80,
        Person = 90,
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
        en,
        it,
        es,
        hr,
        tr,
        fi,
        el,
        fr,
        nl
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldType
    {
        None,
        Int,
        Decimal,
        String,
        Datetime,
        Boolean
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CrisisPhaseType
    {
        None,
        Prevention,
        Preparedness,
        Response,
        PostEvent,
        Environment
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventContextType
    {
        None,
        Home,
        Building,
        InVehicle,
        River,
        Outdoor,
        Everywhere
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum DifficultyType
    {
        Low,
        Medium,
        High
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LayerType
    {
        BurnedArea,
        Delineation,
        Forecast,
        Nowcast,
        RiskMap
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MapRequestStatusType
    {
        RequestSubmitted,
        ContentAvailable,
        ContentNotAvailable,
        Canceled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportContentType
    {
        Submitted = 0,
        Inappropriate = 10,
        Inaccurate = 20,
        Validated = 30
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VisibilityType
    {
        Private = 0,
        Public = 10,
        All = 20,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum VolterOperationType
    {
        Registration,
        OpenIntervention,
        CloseIntervention
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProcessedCodeType
    {
        ElaborazioneTerminataCorretamente = 1,
        CampoObbligatorioNonPresente = 5,
        ErroreParametriInput = 10,
        SistemaDiMateriaChiamanteNonEsistente = 22,
        VolontarioNonEsistente = 30,
        CompitoNonEsistente = 31,
        InsertOUpdateFallito = 33,
        EventoNonEsistente = 38,
        TrovatiPiuInterventiAperti = 39,
        NessunEventoApertoInAltaPriorita = 40,
        CodiceFiscaleCorrispondeAPiuPersone = 41,
        ErroreServizioEventi = 42,
        ImpossibileTrovareAperturaIntervento = 43,
        PresentiPiuApertureInterventoIdentiche = 44,
        AperturaInterventoGiaPresente = 45,
        ErroreGenericoRecuperoInterventi = 46,
        ImpossibileFareMappingAttivita = 47,
        ImpossibileTrovareOrganizzazioneVolontario = 48,
        ImpossibileTrovareCoordinamentoOrganizzazione = 49

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FormatType
    {
        GeoJSON,
        JSON,
        JPEG,
        GeoTIFF,
        NetCDF,
        GeoPNG
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FrequencyType
    {
        Daily,
        OnDemand,
        H6,
        H12
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompetenceType
    {
        Starter,
        Learner,
        Reporter,
        Reviewer,
        Socializer
    }

}
