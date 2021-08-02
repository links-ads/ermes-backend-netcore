using Abp.Application.Services;
using Ermes.Dto.Datatable;
using Ermes.Gamification.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Gamification
{
    public interface IGamificationAppService: IApplicationService
    {
        Task<DTResult<TipDto>> GetTips(GetTipsInput input);
    }
}
