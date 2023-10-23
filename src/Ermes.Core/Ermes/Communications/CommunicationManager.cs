using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Enums;
using Ermes.Missions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Communications
{
    public class CommunicationManager : DomainService
    {
        public IQueryable<Communication> Communications { get { return CommunicationRepository.GetAll().Include(a => a.Creator).Include(a => a.Creator.Organization).Include(a => a.CommunicationReceivers); } }
        public IQueryable<CommunicationReceiver> CommunicationReceivers { get { return CommunicationReceiverRepository.GetAll().Include(a => a.Communication).Include(a => a.Organization); } }
        protected IRepository<Communication> CommunicationRepository { get; set; }
        protected IRepository<CommunicationReceiver> CommunicationReceiverRepository { get; set; }

        public CommunicationManager(
                IRepository<Communication> communicationRepository,
                IRepository<CommunicationReceiver> communicationReceiverRepository
            )
        {
            CommunicationRepository = communicationRepository;
            CommunicationReceiverRepository = communicationReceiverRepository;
        }

        public async Task<Communication> GetCommunicationByIdAsync(int communicationId)
        {
            return await Communications.SingleOrDefaultAsync(a => a.Id == communicationId);
        }

        public async Task<int> CreateOrUpdateCommunicationAsync(Communication co, List<int> receivers = null)
        {
            var coId = await CommunicationRepository.InsertOrUpdateAndGetIdAsync(co);

            if (co.Scope == CommunicationScopeType.Restricted && co.Restriction == CommunicationRestrictionType.Organization && receivers != null)
            {
                foreach (var receiver in receivers)
                {
                    var newItem = new CommunicationReceiver(coId, receiver);
                    CommunicationReceiverRepository.Insert(newItem);
                    co.CommunicationReceivers.Add(newItem);
                }
            }

            return coId;
        }

        public async Task DeleteCommunicationAsync(Communication co)
        {
            await CommunicationRepository.DeleteAsync(co);
        }

        public async Task DeleteCommunicationsByPersonIdAsync(long personId)
        {
            await CommunicationRepository.DeleteAsync(c => c.CreatorUserId.HasValue && c.CreatorUserId.Value == personId);
        }

        public IQueryable<Communication> GetCommunications(DateTime startDate, DateTime endDate)
        {
            NpgsqlRange<DateTime> range = new NpgsqlRange<DateTime>(startDate, endDate);
            return Communications
                    .Where(m => m.Duration.Overlaps(range));
        }
    }
}
