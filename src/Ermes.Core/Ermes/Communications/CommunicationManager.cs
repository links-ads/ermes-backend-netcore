using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Communications
{
    public class CommunicationManager: DomainService
    {
        public IQueryable<Communication> Communications { get { return CommunicationRepository.GetAll().Include(a => a.Creator.Organization); } }
        protected IRepository<Communication> CommunicationRepository { get; set; }

        public CommunicationManager(
                IRepository<Communication> communicationRepository
            )
        {
            CommunicationRepository = communicationRepository;
        }

        public async Task<Communication> GetCommunicationByIdAsync(int communicationId)
        {
            return await Communications.SingleOrDefaultAsync(a => a.Id == communicationId);
        }

        public async Task<int> CreateOrUpdateCommunicationAsync(Communication co)
        {
            return await CommunicationRepository.InsertOrUpdateAndGetIdAsync(co);
        }

        public async Task DeleteCommunicationAsync(Communication co)
        {
            await CommunicationRepository.DeleteAsync(co);
        }
    }
}
