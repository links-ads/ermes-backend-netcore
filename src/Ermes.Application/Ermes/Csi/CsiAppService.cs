using Abp.Authorization;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Csi.Dto;
using Ermes.Operations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Csi
{
    /*
     * Debug purpose service
     */ 
    [ErmesIgnoreApi(true)]
    public class CsiAppService: ErmesAppServiceBase
    {
        private readonly OperationManager _operationManager;
        public CsiAppService(OperationManager operationManager)
        {
            _operationManager = operationManager;
        }

        [AbpAllowAnonymous]
        public async Task<GetOperationsOutput> GetOperations(GetOperationsInput input)
        {
            var res = new GetOperationsOutput();
            List<Operation> operations;

            if (input.PersonId.HasValue && input.PersonId.Value > 0)
            {
                operations = await _operationManager.GetOperationsByPersonIdAsync(input.PersonId.Value);
                res.Operations = ObjectMapper.Map<List<OperationDto>>(operations);
            }
            else if (input.PersonLegacyId.HasValue && input.PersonLegacyId.Value > 0)
            {
                operations = await _operationManager.GetOperationsByPersonLegacyIdAsync(input.PersonLegacyId.Value);
                res.Operations = ObjectMapper.Map<List<OperationDto>>(operations);
            }
            else
                throw new UserFriendlyException("AtLeastOneFilterParam");

            return res;
        }
    }
}
