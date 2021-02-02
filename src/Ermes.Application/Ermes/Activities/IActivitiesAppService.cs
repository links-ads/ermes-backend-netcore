using Abp.Application.Services;
using Ermes.Activities.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Activities
{
    public interface IActivitiesAppService : IApplicationService
    {
        Task<GetActivitiesOutput> GetActivities(GetActivitiesInput input);
    }
}
