using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Helpers;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.MapRequests
{
    public class MapRequestManager : DomainService
    {
        public IQueryable<MapRequest> MapRequests { get { return MapRequestRepository.GetAll().Include(a => a.Creator.Organization); } }
        protected IRepository<MapRequest> MapRequestRepository { get; set; }

        public MapRequestManager(IRepository<MapRequest> mapRequestRepository)
        {
            MapRequestRepository = mapRequestRepository;
        }

        public IQueryable<MapRequest> GetMapRequests(DateTime startDate, DateTime endDate)
        {
            NpgsqlRange<DateTime> range = new NpgsqlRange<DateTime>(startDate, endDate);
            return MapRequests
                    .Where(m => m.Duration.Overlaps(range));
        }

        public async Task<int> CreateOrUpdateMapRequestAsync(MapRequest mr)
        {
            //Code computation
            var lastCode = await MapRequests.Select(a => a.Code).OrderBy(a => a).LastOrDefaultAsync();
            mr.Code = EntityCodeHelper.GetNextCode(ErmesConsts.EntityCode.MapReqeust, lastCode);
            return await MapRequestRepository.InsertOrUpdateAndGetIdAsync(mr);
        }

        public async Task<MapRequest> GetMapRequestByIdAsync(int mapReqId)
        {
            return await MapRequests.SingleOrDefaultAsync(a => a.Id == mapReqId);
        }

        public MapRequest GetMapRequestByCode(string code)
        {
            return MapRequests.SingleOrDefault(a => a.Code == code);
        }

        public async Task DeleteMapRequestAsync(MapRequest mr)
        {
            mr.Status = Enums.MapRequestStatusType.Canceled;
        }
    }
}
