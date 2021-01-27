namespace Ermes.Interfaces
{
    public interface IAppFolders
    {
        string TempFileDownloadFolder { get; }
        string WebLogsFolder { get; set; }
    }
}
