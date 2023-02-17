using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.UI;
using Ermes.Dto;
using Ermes.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Ermes.Web.Controllers
{
    public class FileController: ErmesControllerBase
    {
        private readonly IAppFolders _appFolders;

        public FileController(IAppFolders appFolders)
        {
            _appFolders = appFolders;
        }

        public ActionResult DownloadTempFile(FileDto file)
        {
            var filePath = Path.Combine(_appFolders.TempFileDownloadFolder, file.FileToken);
            if (!System.IO.File.Exists(filePath))
            {
                throw new UserFriendlyException(L("RequestedFileDoesNotExists"));
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            System.IO.File.Delete(filePath);
            return File(fileBytes, file.FileType, file.FileName);
        }
    }
}
