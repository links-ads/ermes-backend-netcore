﻿using Abp.Application.Services.Dto;
using Abp.BackgroundJobs;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Ermes.Attributes;
using Ermes.Auth.Dto;
using Ermes.Authorization;
using Ermes.Communications;
using Ermes.Configuration;
using Ermes.Dto;
using Ermes.Dto.Datatable;
using Ermes.Enums;
using Ermes.EventHandlers;
using Ermes.ExternalServices.Csi;
using Ermes.Gamification;
using Ermes.Gamification.Dto;
using Ermes.Linq.Extensions;
using Ermes.MapRequests;
using Ermes.Missions;
using Ermes.Notifications;
using Ermes.Operations;
using Ermes.Organizations;
using Ermes.Organizations.Dto;
using Ermes.Persons;
using Ermes.Profile.Dto;
using Ermes.Reports;
using Ermes.Roles;
using Ermes.Teams;
using FusionAuthNetCore;
using io.fusionauth.domain;
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
        private readonly GamificationManager _gamificationManager;
        private readonly TeamManager _teamManager;
        private readonly OrganizationManager _organizationManager;
        private readonly NotificationManager _notificationManager;
        private readonly OperationManager _operationManager;
        private readonly ReportManager _reportManager;
        private readonly MapRequestManager _mapRequestManager;
        private readonly CommunicationManager _communicationManager;
        private readonly IOptions<FusionAuthSettings> _fusionAuthSettings;
        private readonly IOptions<ErmesSettings> _ermesSettings;
        private readonly ErmesPermissionChecker _permissionChecker;
        private readonly CsiManager _csiManager;
        private readonly IBackgroundJobManager _jobManager;
        public ProfileAppService(ErmesAppSession session,
                    PersonManager personManger,
                    MissionManager missionManager,
                    GamificationManager gamificationManager,
                    TeamManager teamManager,
                    OrganizationManager organizationManager,
                    NotificationManager notificationManager,
                    OperationManager operationManager,
                    ReportManager reportManager,
                    MapRequestManager mapRequestManager,
                    CommunicationManager communicationManager,
                    ErmesPermissionChecker permissionChecker,
                    IOptions<FusionAuthSettings> fusionAuthSettings,
                    IOptions<ErmesSettings> ermesSettings,
                    CsiManager csiManager,
                    IBackgroundJobManager jobManager)
        {
            _session = session;
            _personManager = personManger;
            _fusionAuthSettings = fusionAuthSettings;
            _missionManager = missionManager;
            _teamManager = teamManager;
            _permissionChecker = permissionChecker;
            _organizationManager = organizationManager;
            _ermesSettings = ermesSettings;
            _csiManager = csiManager;
            _gamificationManager = gamificationManager;
            _jobManager = jobManager;
            _notificationManager = notificationManager;
            _operationManager = operationManager;
            _reportManager = reportManager;
            _communicationManager = communicationManager;
            _mapRequestManager = mapRequestManager;
        }

        #region Private
        private async Task<PagedResultDto<OrganizationDto>> InternalGetOrganizations(GetOrganizationsInput input)
        {
            PagedResultDto<OrganizationDto> result = new PagedResultDto<OrganizationDto>();
            var query = _organizationManager.Organizations;

            if (input != null && input.ParentId.HasValue && input.ParentId.Value > 0)
                query = query.Where(o => o.ParentId.HasValue && o.ParentId.Value == input.ParentId.Value);
            else
                query = query.Where(o => !o.ParentId.HasValue);

            result.TotalCount = await query.CountAsync();

            if (input?.Order != null && input.Order.Count == 0)
            {
                query = query.OrderBy(a => a.Name);
                query = query.PageBy(input);
            }
            else
            {
                query = query
                        .DTOrderedBy(input)
                        .PageBy(input);
            }

            var items = await query.ToListAsync();
            result.Items = ObjectMapper.Map<List<OrganizationDto>>(items);
            return result;
        }

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

            // Security commented after issue #164
            //User now can change the organization he belongs to
            //if (_session.LoggedUserPerson.OrganizationId.HasValue && person.OrganizationId != _session.LoggedUserPerson.OrganizationId)
            //    throw new UserFriendlyException(L("Forbidden_DifferentOrganizations"));
            // TODO-Security: Add Check: Same User or Role allowing Other User Access
            if (_session.UserId != person.Id) // for the moment allow only same user
                throw new UserFriendlyException(L("Forbidden_InsufficientRole"));

            var response = await client.RetrieveUserAsync(person.FusionAuthUserGuid);
            if (response.WasSuccessful())
            {
                if (person.Username == null)
                    person.Username = response.successResponse.user.username;
                if (person.Email == null)
                    person.Email = response.successResponse.user.email;
                return new GetProfileOutput()
                {
                    Profile = await GetProfileInternal(person, response.successResponse.user, _personManager, _missionManager, _gamificationManager, _session, _jobManager)
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
            Output: the profile with updated information
            "
        )]
        [UnitOfWork(false)]
        public virtual async Task<UpdateProfileOutput> UpdateProfile(UpdateProfileInput input)
        {
            if (input.User.Timezone == null || input.User.Timezone.Length < 2)
                input.User.Timezone = AppConsts.DefaultTimezone;

            Person person = await _personManager.GetPersonByIdAsync(input.PersonId ?? _session.UserId.Value);
            var rolesToAssign = await GetRolesAndCheckOrganizationAndTeam(input.User.Roles, input.OrganizationId, input.TeamId, input.PersonId, _personManager, _organizationManager, _teamManager, _session, _permissionChecker);
            var user = await UpdateUserInternalAsync(input.User, _fusionAuthSettings, rolesToAssign.Select(r => r.Name).ToList());

            int? legacyId;
            if (
                _ermesSettings.Value != null &&
                _ermesSettings.Value.ErmesProject == AppConsts.Ermes_Faster &&
                input.OrganizationId.HasValue &&
                !person.LegacyId.HasValue &&
                input.TaxCode != null
            )
            {
                var refOrg = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId.Value);
                var housePartner = await SettingManager.GetSettingValueAsync(AppSettings.General.HouseOrganization);
                if (refOrg.Name == housePartner || (refOrg.ParentId.HasValue && refOrg.Parent.Name == housePartner))
                {
                    legacyId = await _csiManager.SearchVolontarioAsync(input.TaxCode, person.Id);
                    if (legacyId.HasValue && legacyId.Value >= 0)
                        person.LegacyId = legacyId;
                    else
                    {
                        await CurrentUnitOfWork.SaveChangesAsync();
                        throw new UserFriendlyException(L("InvalidVolterTaxCode"));
                    }
                }
            }
            bool oldFirstLoginValue = person.IsFirstLogin, oldIsNewUserValue = person.IsNewUser;
            
            person = await CreateOrUpdatePersonInternalAsync(person, user, input.OrganizationId, input.TeamId, input.IsFirstLogin, input.IsNewUser, rolesToAssign, _personManager);

            if (rolesToAssign.Select(r => r.Name).Contains(AppRoles.CITIZEN))
            {
                var list = new List<(EntityWriteAction Action, string NewValue, int EarnedPoints)>();
                if (oldFirstLoginValue != person.IsFirstLogin && !person.IsFirstLogin) //send notification, the user has completed the tutorial
                {
                    var action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.COMPLETE_WIZARD);
                    list = await _gamificationManager.UpdatePersonGamificationProfileAsync(person.Id, action.Name, null);
                    list.Add((EntityWriteAction.CompleteWizard, action.Name, action.Points));
                }

                if (oldIsNewUserValue != person.IsNewUser && !person.IsNewUser)
                {
                    var action = await _gamificationManager.GetActionByNameAsync(ErmesConsts.GamificationActionConsts.FIRST_LOGIN);
                    list.AddRange(await _gamificationManager.UpdatePersonGamificationProfileAsync(person.Id, action.Name, null));
                    list.Add((EntityWriteAction.FirstLogin, action.Name, action.Points));
                }

                foreach (var item in list)
                {
                    NotificationEvent<GamificationNotificationDto> gamNotification = new NotificationEvent<GamificationNotificationDto>(0,
                    _session.LoggedUserPerson.Id,
                    new GamificationNotificationDto()
                    {
                        PersonId = _session.LoggedUserPerson.Id,
                        ActionName = item.Action.ToString(),
                        NewValue = item.NewValue,
                        EarnedPoints = item.EarnedPoints
                    },
                    item.Action,
                    true);
                    await _jobManager.EnqueueEventAsync(gamNotification);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            if (user != null)
            {
                return new UpdateProfileOutput()
                {
                    Profile = await GetProfileInternal(person, user, _personManager, _missionManager, _gamificationManager, _session, _jobManager)
                };
            }
            else
                throw new UserFriendlyException(L("InvalidUser"));
        }

        [OpenApiOperation("Delete profile",
            @"
                Remove the person from the system
                Input: 
                    - Guid provided by FusionAuth
                    - HardDelete: if true, delete every info associated to the person, if false, deactivate account on FusionAuth
                Output: true is the operation ends successfully
                Notes: a user can be deleted by org manager of the same organization or belonging to a parent organization
            "
        )]
        public virtual async Task<bool> DeleteProfile(DeleteProfileInput input)
        {
            Guid refGuid;

            //The operation can be performed by:
            //  - admin
            //  - the user on his own account
            if (input == null || input.Id == null || input.Id == Guid.Empty)
                refGuid = _session.FusionAuthUserGuid.Value;
            else
            {
                var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Profiles.Profile_CanDelete);
                if (!hasPermission)
                    throw new UserFriendlyException("MissingPermission");

                refGuid = input.Id;
            }
            
            var person = await _personManager.GetPersonByFusionAuthUserGuidAsync(refGuid);

            if (person.OrganizationId.HasValue)
            {
                var loggedPerson = _session.LoggedUserPerson;
                if (loggedPerson.OrganizationId != person.OrganizationId.Value && loggedPerson.OrganizationId != person.Organization.ParentId)
                    throw new UserFriendlyException("EntityOutsideOrganization");
            }

            return await DeleteUserInternalAsync(
                refGuid, 
                person, 
                input.HardDelete, 
                _notificationManager, 
                _operationManager, 
                _reportManager, 
                _communicationManager, 
                _mapRequestManager, 
                _missionManager, 
                _personManager, 
                _gamificationManager, 
                _fusionAuthSettings
            );
        }

        [OpenApiOperation("Reactivate profile",
            @"
                Reactivate an already existing profile
                Input: 
                    - Email address of the profile to be reactiveted
                Output: ProfileDto object
                Notes: a user can be reactivated by org manager of the same organization or belonging to a parent organization
            "
        )]
        public virtual async Task<GetProfileOutput> ReactivateProfile(ReactivateProfileInput input)
        {
            var person = _personManager.GetPersonByEmail(input.Email);
            if (person == null)
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            if (person.FusionAuthUserGuid == Guid.Empty)
                throw new UserFriendlyException(L("InvalidGuid"));

            var hasPermission = _permissionChecker.IsGranted(_session.Roles, AppPermissions.Profiles.Profile_CanReactivate);
            if (!hasPermission)
                throw new UserFriendlyException("MissingPermission");

            //no additional checks needed for citizens
            if (person.OrganizationId.HasValue) {
                var loggedPerson = _session.LoggedUserPerson;
                if (loggedPerson.OrganizationId != person.OrganizationId.Value && loggedPerson.OrganizationId != person.Organization.ParentId)
                    throw new UserFriendlyException("EntityOutsideOrganization");
            }

            if (person.IsActive)
                throw new UserFriendlyException(L("PersonIsActive"));

            var client = FusionAuth.GetFusionAuthClient(_fusionAuthSettings.Value);

            var response = await client.ReactivateUserAsync(person.FusionAuthUserGuid);
            if (!response.WasSuccessful())
            {
                var fa_error = FusionAuth.ManageErrorResponse(response);
                throw new UserFriendlyException(fa_error.ErrorCode, fa_error.HasTranslation ? L(fa_error.Message) : fa_error.Message);
            }

            person.IsActive = true;
            return new GetProfileOutput()
            {
                Profile = await GetProfileInternal(person, response.successResponse.user, _personManager, _missionManager, _gamificationManager, _session, _jobManager)
            };

        }

        [OpenApiOperation("Update Registration Token",
            @"
                Update Registration Token associated to the current logged user.
                This token will be used by push notification service
                Input: UpdateRegistrationTokenInput object with the token
                Output: true if the operation has been successfully executed
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

        //[OpenApiOperation("Update Person preferred languages",
        //    @"
        //        [Deprecated]
        //        Update Person Preferred Languages
        //        Input: UpdatePreferredLanguagesInput Dto object with the full list of preferred languages in Iso2Code format
        //        Output: true if the operation has been successfully executed
        //    "
        //)]
        //public virtual async Task<bool> UpdatePreferredLanguages(UpdatePreferredLanguagesInput input)
        //{
        //    Person person = await _personManager.GetPersonByIdAsync(_session.UserId.Value);

        //    // Validation
        //    if (person == null)
        //        throw new UserFriendlyException(L("InvalidPersonId", _session.UserId.Value));

        //    var userToUpdate = await GetUserAsync(person.FusionAuthUserGuid);
        //    if (person.Username == null)
        //        person.Username = userToUpdate.username;

        //    userToUpdate.preferredLanguages = input.PreferredLanguages;
        //    var user = await UpdateUserInternalAsync(ObjectMapper.Map<UserDto>(userToUpdate), _fusionAuthSettings);
        //    return user != null;
        //}


        [OpenApiOperation("Get Organization List",
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
                    In addition to pagination parameters, there are additional properties for organization filtering:
                                - ParentId: id of an Organization. If null, only father organizations are returned, otherwise tue API returns the children of the specified parent
                Output: list of OrganizationDto elements
                N.B: this API is used by chatbot application during the startup phase --> no auth required.
            "
        )]
        public async Task<DTResult<OrganizationDto>> GetOrganizations(GetOrganizationsInput input)
        {
            PagedResultDto<OrganizationDto> result = await InternalGetOrganizations(input);
            return new DTResult<OrganizationDto>(0, result.TotalCount, result.Items.Count, result.Items.ToList());
        }

        [OpenApiOperation("Change Organization",
            @"
                Allow user to change his organization
                Input:
                    - OrganizationId: Id of the new organization
                    - TaxCode: tax code of the user, if the organization requires it
                Output: a ProfileDto object
            "
        )]
        public virtual async Task<GetProfileOutput> ChangeOrganization(ChangeOrganizationInput input)
        {
            var org = await _organizationManager.GetOrganizationByIdAsync(input.OrganizationId);

            if (org == null)
                throw new UserFriendlyException(L("InvalidEntityId", "Organization", input.OrganizationId));

            Person person = await _personManager.GetPersonByIdAsync(_session.LoggedUserPerson.Id);

            //When user changes org:
            // 1. he cannot be coordinator of missions belonging to his old organization
            // 2. his reports cannot be associated to a mission belonging to his old organization
            var orgMissions = _missionManager.Missions.Where(m => m.OrganizationId == person.OrganizationId).ToList();

            var coordinatedMissions = orgMissions.Where(m => m.CoordinatorPersonId.HasValue && m.CoordinatorPersonId.Value == person.Id).ToList();
            coordinatedMissions = coordinatedMissions.Select(m => { m.CoordinatorPersonId = null; return m; }).ToList();

            var orgMissionIds = orgMissions.Select(m => m.Id).ToList();
            var relatedReports = _reportManager.Reports.Where(r => r.CreatorUserId == person.Id && r.RelativeMissionId.HasValue && orgMissionIds.Contains(r.RelativeMissionId.Value)).ToList();
            relatedReports = relatedReports.Select(r => { r.RelativeMissionId = null; return r; }).ToList();

            if (org.MembersHaveTaxCode)
            {
                if (input.TaxCode == null || input.TaxCode == string.Empty)
                    throw new UserFriendlyException(L("InvalidTaxCode"));

                int? legacyId = await _csiManager.SearchVolontarioAsync(input.TaxCode, person.Id);
                if (legacyId.HasValue && legacyId.Value >= 0)
                {
                    var existingPerson = _personManager.GetPersonByLegacyId(legacyId.Value);
                    if (existingPerson != null && existingPerson.Id != person.Id)
                        throw new UserFriendlyException(L("TaxCodeAlreadyInUse"));
                    person.LegacyId = legacyId;
                }
                else
                    throw new UserFriendlyException(L("InvalidVolterTaxCode"));
            }
            else
                person.LegacyId = null;

            person.OrganizationId = input.OrganizationId;
            person.TeamId = null;
            await CurrentUnitOfWork.SaveChangesAsync();
            return await GetProfileById(new IdInput<long>() { Id = _session.UserId.Value });
        }
    }
}
