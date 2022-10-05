using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Enums;
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
        public IQueryable<MapRequest> MapRequests { get { return MapRequestRepository.GetAll().Include(a => a.Creator.Organization).Include(a => a.MapRequestLayers); } }
        protected IRepository<MapRequest> MapRequestRepository { get; set; }
        protected IRepository<MapRequestLayer> MapRequestLayersRepository { get; set; }

        public MapRequestManager(IRepository<MapRequest> mapRequestRepository, IRepository<MapRequestLayer> mapRequestLayersRepository)
        {
            MapRequestRepository = mapRequestRepository;
            MapRequestLayersRepository = mapRequestLayersRepository;
        }

        public IQueryable<MapRequest> GetMapRequests(DateTime startDate, DateTime endDate)
        {
            NpgsqlRange<DateTime> range = new NpgsqlRange<DateTime>(startDate, endDate);
            return MapRequests
                    .Where(m => m.Duration.Overlaps(range));
        }

        public async Task<int> CreateOrUpdateMapRequestAsync(MapRequest mr, List<int> dataTypeIds)
        {
            //Code computation
            var lastCode = await MapRequests.Select(a => a.Code).OrderBy(a => a).LastOrDefaultAsync();
            mr.Code = EntityCodeHelper.GetNextCode(ErmesConsts.EntityCode.MapReqeust, lastCode);
            var mapRequestId = await MapRequestRepository.InsertOrUpdateAndGetIdAsync(mr);

            foreach (var dataTypeId in dataTypeIds)
            {
                await MapRequestLayersRepository.InsertAsync(new MapRequestLayer()
                {
                    LayerDataTypeId = dataTypeId,
                    MapRequestCode = mr.Code,
                    Status = LayerImportStatusType.Created
                });
            }

            return mapRequestId;
        }

        public async Task<MapRequest> GetMapRequestByIdAsync(int mapReqId)
        {
            return await MapRequests.SingleOrDefaultAsync(a => a.Id == mapReqId);
        }

        public MapRequest GetMapRequestByCode(string code)
        {
            return MapRequests.SingleOrDefault(a => a.Code == code);
        }
        public async Task<MapRequest> GetMapRequestByCodeAsync(string code)
        {
            return await MapRequests.SingleOrDefaultAsync(a => a.Code == code);
        }

        public async Task DeleteMapRequestAsync(MapRequest mr)
        {
            mr.Status = Enums.MapRequestStatusType.Canceled;
        }

        public async Task DeleteMapRequestsByPersonIdAsync(long personId)
        {
            await MapRequestLayersRepository.DeleteAsync(ml => ml.CreatorUserId.HasValue && ml.CreatorUserId.Value == personId);
            await MapRequestRepository.DeleteAsync(mr => mr.CreatorUserId.HasValue && mr.CreatorUserId.Value == personId);
        }
    }
}
