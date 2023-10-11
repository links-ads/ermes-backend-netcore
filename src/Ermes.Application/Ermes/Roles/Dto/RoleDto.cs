using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Ermes.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleDto
    {
        public int Id { get; set; }
        [StringLength(Role.MAX_NAME_LENGTH)]
        public string Name { get; set; }
        [StringLength(Role.MAX_DESCRIPTION_LENGTH)]
        public string Description { get; set; }
        public bool Default { get; set; }
        public bool SuperRole { get; set; }
    }
}
