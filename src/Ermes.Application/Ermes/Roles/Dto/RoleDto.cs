using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Ermes.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Default { get; set; }
        public bool SuperRole { get; set; }
    }
}
