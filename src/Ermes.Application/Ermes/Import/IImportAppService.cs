﻿using Ermes.Import.Dto;
using Ermes.Interfaces;
using Ermes.Roles.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Import
{
    public interface IImportAppService : IBackofficeApi
    {
        Task<ImportResultDto> ImportActivities(IFormFile file);
        Task<bool> ImportCompetenceAreas();
        Task<ImportResultDto> ImportCategories(IFormFile file);
        Task<ImportResultDto> ImportUsers(IFormFile file);
        Task<ImportResultDto> ImportTips(IFormFile file);
        Task<ImportResultDto> ImportQuizzes(IFormFile file);
        Task<ImportResultDto> ImportAnswers(IFormFile file);
        Task<ImportResultDto> ImportLayers(IFormFile file);
        Task<ImportResultDto> ImportGamificationActions(IFormFile file);
    }
}
