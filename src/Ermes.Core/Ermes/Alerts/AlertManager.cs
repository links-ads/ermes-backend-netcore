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
        public IQueryable<Alert> AlertWithAreas { get { return AlertRepository.GetAll().Include(a => a.Info).Include(a => a.AlertAreaOfInterest); } }
        public IQueryable<Alert> Alerts { get { return AlertRepository.GetAll().Include(a => a.Info); } }
        public IQueryable<AlertAreaOfInterest> AlertAreasOfInterest { get { return AlertAreaOfInterestRepository.GetAll(); } }
        public IQueryable<CapInfo> CapInfoList  { get { return CapInfoRepository.GetAll(); } }
        protected IRepository<Alert> AlertRepository { get; set; }
        protected IRepository<AlertAreaOfInterest> AlertAreaOfInterestRepository { get; set; }
        protected IRepository<CapInfo> CapInfoRepository { get; set; }

        public AlertManager(IRepository<Alert> alertRepository, IRepository<CapInfo> capInfoRepository, IRepository<AlertAreaOfInterest> alertAreaOfInterestRepository)
        {
            AlertRepository = alertRepository;
            CapInfoRepository = capInfoRepository;
            AlertAreaOfInterestRepository = alertAreaOfInterestRepository;
        }

        public async Task<List<Alert>> GetAlertsAsync()
        {
            return await Alerts.ToListAsync();
        }

        public int InsertAlertAndGetId(Alert alert)
        {
            return AlertRepository.InsertAndGetId(alert);
        }

        public int InsertAlertAreaOfInterestAndGetId(AlertAreaOfInterest alertAreaOfInterest)
        {
            return AlertAreaOfInterestRepository.InsertAndGetId(alertAreaOfInterest);
        }

        public int InsertCapInfoAndGetId(CapInfo info)
        {
            return CapInfoRepository.InsertAndGetId(info);
        }

        public async Task<Alert> GetAlertByIdAsync(int alertId)
        {
            return await AlertWithAreas.SingleOrDefaultAsync(a => a.Id == alertId);
        }
    }
}
