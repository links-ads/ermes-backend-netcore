using Ermes.Auth.Dto;
using Ermes.Persons;
using io.fusionauth.domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Import.Dto
{
    public class ImportUsersDto : ImportResultDto
    {
        public List<Tuple<UserDto,Person>> Accounts { get; set; }
    }
}
