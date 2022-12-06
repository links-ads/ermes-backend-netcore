using Abp.Domain.Entities;
using Ermes.Organizations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Ermes.Communications
{
    /// <summary>
    /// In case Restriction == Organization, this table contains
    /// the list of the organization whose members will receive the communication.
    /// If empty, the communication will be sent to creator organization + children organizations
    /// </summary>
    [Table("communication_receivers")]
    public class CommunicationReceiver : Entity
    {
        public CommunicationReceiver(int communicationId, int organizationId)
        {
            CommunicationId = communicationId;
            OrganizationId = organizationId;
        }


        [ForeignKey("CommunicationId")]
        public virtual Communication Communication { get; set; }
        public int CommunicationId { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }
        public int OrganizationId { get; set; }

    }
}
