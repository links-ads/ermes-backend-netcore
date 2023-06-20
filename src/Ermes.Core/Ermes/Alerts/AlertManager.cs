using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Alerts
{
    public class AlertManager : DomainService
    {
        public IQueryable<Alert> Alerts { get { return AlertRepository.GetAll().Include(a => a.Info); } }
        public IQueryable<CapInfo> CapInfoList  { get { return CapInfoRepository.GetAll(); } }
        protected IRepository<Alert> AlertRepository { get; set; }
        protected IRepository<CapInfo> CapInfoRepository { get; set; }

        public AlertManager(IRepository<Alert> alertRepository, IRepository<CapInfo> capInfoRepository)
        {
            AlertRepository = alertRepository;
            CapInfoRepository = capInfoRepository;
        }

        public async Task<List<Alert>> GetAlertsAsync()
        {
            return await Alerts.ToListAsync();
        }

        public int InsertAlertAndGetId(Alert alert)
        {
            return AlertRepository.InsertAndGetId(alert);
        }

        public int InsertCapInfoAndGetId(CapInfo info)
        {
            return CapInfoRepository.InsertAndGetId(info);
        }

        public async Task<Alert> GetAlertByIdAsync(int alertId)
        {
            return await Alerts.SingleOrDefaultAsync(a => a.Id == alertId);
        }
    }
}
