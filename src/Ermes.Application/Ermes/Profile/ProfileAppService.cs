﻿using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Linq.Extensions;
using Ermes.Missions;
using Ermes.Organizations;
using Ermes.Persons;
using Ermes.Profile.Dto;
using Ermes.Teams;
using FusionAuthNetCore;
using io.fusionauth.domain;
using io.fusionauth.domain.api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ermes.Profile
{
    [ErmesAuthorize]
    public class ProfileAppService : ErmesAppServiceBase, IProfileAppService
    {
        private readonly ErmesAppSession _session;
        private readonly PersonManager _personManager;
        private readonly MissionManager _missionManager;
        private readonly TeamManager _teamManager;
        private readonly OrganizationManager _organizationManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;

        public ProfileAppService(ErmesAppSession session,
                    PersonManager personManger,
                    MissionManager missionManager,
                    TeamManager teamManager,
                    OrganizationManager organizationManager,
                    IOptions<FusionAuthSettings> fusionAuthSettings)
        {
            _session = session;
            _personManager = personManger;
            _fusionAuthSettings = fusionAuthSettings;
            _missionManager = missionManager;
            _teamManager = teamManager;
            _organizationManager = organizationManager;
        }

        #region Private
        private async Task<PagedResultDto<PersonDto>> InternalGetOrganizationMemebers(GetOrganizationMembersInput input, bool filterByOrganization = true)
        {
            PagedResultDto<PersonDto> result = new PagedResultDto<PersonDto>();

            IQueryable<Person> query = _personManager.Persons.Include(a => a.Team).Include(a => a.Organization);

            query = query.DTFilterBy(input);

            var currentUserPerson = _session.LoggedUserPerson;

            if (filterByOrganization && currentUserPerson.OrganizationId.HasValue)
                query = query.DataOwnership(new List<int>() { currentUserPerson.OrganizationId.Value });

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderByDescending(a => a.Id);
                query = query.PageBy(input);
            }
            else
            {
                query = query.DTOrderedBy(input)
                    .PageBy(input);
            }

            List<Person> items = await query.ToListAsync();

            result.Items = ObjectMapper.Map<List<PersonDto>>(items);

            return result;
        }
        private async Task<User> UpdateUserAsync(User user)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var request = new UserRequest()
            {
                user = user,
                sendSetPasswordEmail = false,
                skipVerification = true
            };

            var response = await client.UpdateUserAsync(user.id, request);
            if (response.WasSuccessful())
            {
                Logger.Info("Ermes: Update User: " + user.username);
                return response.successResponse.user;
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }
        private async Task<User> GetUserAsync(Guid userGuid)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);
            var response = await client.RetrieveUserAsync(userGuid);
            if (response.WasSuccessful())
                return response.successResponse.user;
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }
        #endregion

        [OpenApiOperation("Get profile by id",
            @"
                Get profile information of the specified person
                Input: none
                Output: a ProfileDto object
            "
        )]
        public virtual async Task<GetProfileOutput> GetProfileById(IdInput<long> input)
        {
            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            Person person = await _personManager.GetPersonByIdAsync(input.Id);

            if (person == null)
                throw new UserFriendlyException(L("InvalidPersonId", input.Id));

            // Security
            if (_session.LoggedUserPerson.OrganizationId.HasValue && person.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));
            // TODO-Security: Add Check: Same User or Role allowing Other User Access
            if (_session.UserId != person.Id) // for the moment allow only same user
                throw new UserFriendlyException(L("Forbidden_InsufficientRole"));

            var response = await client.RetrieveUserAsync(person.FusionAuthUserGuid);
            if (response.WasSuccessful())
            {
                if (person.Username == null)
                    person.Username = response.successResponse.user.username;
                return new GetProfileOutput()
                {
                    Profile = await GetProfileInternal(person, response.successResponse.user, _personManager, _missionManager)
                };
            }
            else
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }
        }

        [OpenApiOperation("Update Profile",
            @"
            Replace all profile information with the new values contained in the request. Note that if the input is not complete or contains null values, the corresponding fields in the profile informations will be overwritten to null - only exception is the password field, that can be null in non-password-changing updates. Ignores roles, throws exception on organization change attempt.
            Input: UpdateProfileInput Dto object
            Output: UserId of updated profile
            "
        )]
        
        public virtual async Task<UpdateProfileOutput> UpdateProfile(UpdateProfileInput input)
        {
            Person person = await _personManager.GetPersonByIdAsync(input.PersonId ?? _session.UserId.Value);
            // Validation
            if (person == null)
                throw new UserFriendlyException(L("InvalidPersonId", input.PersonId));
            if (input.User.Timezone == null)
                throw new UserFriendlyException(L("MissingRequiredField", "Timezone"));
            if (input.OrganizationId != person.OrganizationId)
                throw new UserFriendlyException(L("CannotChangeOrganization"));

            await CheckOrganizationAndTeam(_organizationManager, _teamManager, input.OrganizationId, input.TeamId);
            person.TeamId = input.TeamId;

            // Security
            if (_session.LoggedUserPerson.OrganizationId.HasValue && person.OrganizationId != _session.LoggedUserPerson.OrganizationId)
                throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));
            // TODO-Security: Add Check: Same User or Role allowing Other User Access
            if (_session.UserId != person.Id) // for the moment allow only same user
                throw new UserFriendlyException(L("Forbidden_InsufficientRole"));

            // Update database fields
            person.Username = input.User.Username;
            person.IsFirstLogin = input.IsFirstLogin;
            input.User.Id = person.FusionAuthUserGuid;
            var user = await UpdateUserAsync(ObjectMapper.Map<User>(input.User));
            if (user != null)
            {
                return new UpdateProfileOutput()
                {
                    Profile = await GetProfileInternal(person, user, _personManager, _missionManager),
                };
            }
            else
                throw new UserFriendlyException(L("InvalidUser"));
        }

        [OpenApiOperation("Delete profile",
            @"
                To be implemented
            "
        )]
        public virtual async Task<bool> DeleteProfile(IdInput<int> input)
        {
            throw new NotImplementedException();
        }

        [OpenApiOperation("Update Registration Token",
            @"
                Update Registration Token associated to the current logged user.
                This token will be used by push notification service
                Input: UpdateRegistrationTokenInput object with the token
                Output: true if the operation has been excuted successfully
            "
        )]
        public virtual async Task<bool> UpdateRegistrationToken(UpdateRegistrationTokenInput input)
        {
            var person = _session.FusionAuthUserGuid.HasValue ? _personManager.GetPersonByFusionAuthUserGuid(_session.FusionAuthUserGuid.Value) : null;
            if (person == null)
                throw new UserFriendlyException(L("InvalidPersonId", _session.FusionAuthUserGuid));
            if (input.RegistrationToken.IsNullOrWhiteSpace())
                throw new UserFriendlyException(L("InvalidRegistrationToken"));
            person.RegistrationToken = input.RegistrationToken;

            return true;
        }

        [OpenApiOperation("Get profile",
            @"
                Get profile information of the person who is making the request
                Input: none
                Output: a ProfileDto object
            "
        )]
        public virtual async Task<GetProfileOutput> GetProfile()
        {
            return await GetProfileById(new IdInput<long>(){Id=_session.UserId.Value});
        }

        [OpenApiOperation("Get Organization Members",
            @"
                This is a server-side paginated API
                Input: use the following properties to filter result list:
                    - Draw: Draw counter. This is used by DataTables to ensure that the Ajax returns from server-side processing requests are drawn in sequence by 
                        DataTables (Ajax requests are asynchronous and thus can return out of sequence)
                    - MaxResultCount: number of records that the table can display in the current draw (must be >= 0)
                    - SkipCount: paging first record indicator. This is the start point in the current data set (0 index based - i.e. 0 is the first record)
                    - Search: 
                               - value: global search value
                               - regex: true if the global filter should be treated as a regular expression for advanced searching, false otherwise
                    - Order (is a list, for multi-column sorting):
                                - column: name of the column to which sorting should be applied
                                - dir: sorting direction
                Output: list of PersonDto elements

                N.B.: A person has visibility only on persons belonging to his organization
            "
        )]
        public virtual async Task<DTResult<PersonDto>> GetOrganizationMembers(GetOrganizationMembersInput input)
        {
            PagedResultDto<PersonDto> result = await InternalGetOrganizationMemebers(input);
            return new DTResult<PersonDto>(input.Draw, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Update Person preferred languages",
            @"
                [Deprecated]
                Update Person Preferred Languages
                Input: UpdatePreferredLanguagesInput Dto object with the full list of preferred languages in Iso2Code format
                Output: true if the operation has been excuted successfully
            "
        )]
        public virtual async Task<bool> UpdatePreferredLanguages(UpdatePreferredLanguagesInput input)
        {
            Person person = await _personManager.GetPersonByIdAsync(_session.UserId.Value);

            // Validation
            if (person == null)
                throw new UserFriendlyException(L("InvalidPersonId", _session.UserId.Value));

            var userToUpdate = await GetUserAsync(person.FusionAuthUserGuid);
            if (person.Username == null)
                person.Username = userToUpdate.username;

            userToUpdate.preferredLanguages = input.PreferredLanguages;
            var user = await UpdateUserAsync(userToUpdate);
            return user != null;
        }
    }
}
