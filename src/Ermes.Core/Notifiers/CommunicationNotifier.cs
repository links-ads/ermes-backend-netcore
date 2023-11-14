using Ermes.Communications;
using Ermes.Enums;
using Ermes.GeoJson;
using Ermes.Persons;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public class CommunicationNotifier: ErmesDomainServiceBase
    {
        private readonly NotifierService _notifierService;
        private readonly PersonManager _personManager;
        private readonly CommunicationManager _communicationManager;
        private readonly IGeoJsonBulkRepository _geoJsonBulkRepository;
        public CommunicationNotifier(NotifierService notifierService, PersonManager personManager, CommunicationManager communicationManager, IGeoJsonBulkRepository geoJsonBulkRepository)
        {
            _notifierService = notifierService;
            _personManager = personManager;
            _communicationManager = communicationManager;
            _geoJsonBulkRepository = geoJsonBulkRepository;
        }
        public async Task SendCommunication(EntityWriteAction entityWriteAction, int communicationId, long creatorId, string message, List<int> organizationReceiverIds = null)
        {
            await _notifierService.SendBusNotification(creatorId, communicationId, message, entityWriteAction, EntityType.Communication);

            string titleKey = (entityWriteAction == EntityWriteAction.Create) ? "Notification_Communication_Create_Title" : "Notification_Communication_Update_Title";
            string[] bodyParams = new string[] { message };

            List<long> personIdList;
            try
            {
                //Need to filter receivers by the Area of Interest of the communication
                var comm = await _communicationManager.GetCommunicationByIdAsync(communicationId);

                //Exclude persons in status = Off; citizens are by default in status = Ready
                var statusTypes = new List<ActionStatusType>() { ActionStatusType.Active, ActionStatusType.Moving, ActionStatusType.Ready };

                //Include persons that can be considered active 24h before communication lower bound
                var items = _geoJsonBulkRepository.GetPersonActions(comm.Duration.LowerBound.AddHours(-24), comm.Duration.UpperBound, organizationReceiverIds?.ToArray(), statusTypes, null, comm.AreaOfInterest, null, "en", comm.Scope, comm.Restriction);

                var actions = JsonConvert.DeserializeObject<PersonActionList>(items);
                actions.PersonActions ??= new List<PersonActionSharingPosition>();
                personIdList = actions.PersonActions.Select(a => a.PersonId).ToList();
            }
            catch
            {
                personIdList = null;
            }

            var receivers = _personManager
                                .Persons
                                .Include(p => p.Organization)
                                .Where(p => personIdList != null && personIdList.Contains(p.Id));
            await _notifierService.SendUserNotification(creatorId, receivers, communicationId, ("Notification_Communication_Create_Body", bodyParams), (titleKey, null), entityWriteAction, EntityType.Communication);
        }
    }
}
