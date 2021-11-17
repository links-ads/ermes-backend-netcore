using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Operations
{
    public class OperationManager: DomainService
    {
        public IQueryable<Operation> Operations { get { return OperationRepository.GetAll().Include(a => a.Person); } }
        protected IRepository<Operation> OperationRepository { get; set; }

        public OperationManager(
                IRepository<Operation> operationRepository
            )
        {
            OperationRepository = operationRepository;
        }

        public async Task<List<Operation>> GetOperationsByPersonId(int personId)
        {
            return await Operations.Where(a => a.PersonId == personId).ToListAsync();
        }

        public async Task<List<Operation>> GetOperationsByPersonLegacyId(int personLegacyId)
        {
            return await Operations.Where(a => a.PersonLegacyId == personLegacyId).ToListAsync();
        }

        public async Task<int> InsertOrUpdateOperationAsync(Operation operation)
        {
            return await OperationRepository.InsertOrUpdateAndGetIdAsync(operation);
        }
    }
}
