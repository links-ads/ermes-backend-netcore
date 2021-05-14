using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Ermes.Persons;
using Ermes.Roles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Permissions
{
    public class PermissionManager : DomainService
    {
        public IQueryable<Role> Roles { get { return RoleRepository.GetAll(); } }
        public IQueryable<ErmesPermission> Permissions { get { return PermissionRepository.GetAll(); } }
        public IQueryable<PersonRole> PersonRoles { get { return PersonRoleRepository.GetAll(); } }
        protected IRepository<ErmesPermission> PermissionRepository { get; set; }
        protected IRepository<Role> RoleRepository { get; set; }
        protected IRepository<PersonRole> PersonRoleRepository { get; set; }

        public PermissionManager(
                    IRepository<ErmesPermission> permissionRepository,
                    IRepository<Role> roleRepository,
                    IRepository<PersonRole> personRoleRepository)
        {
            PermissionRepository = permissionRepository;
            RoleRepository = roleRepository;
            PersonRoleRepository = personRoleRepository;
        }

        public bool IsPermissionGrantedForRoles(string[] roles, string permission)
        {
            var roleIdList = Roles
                .Where(r => roles.Contains(r.Name))
                .Select(a => a.Id)
                .ToList();

            return Permissions
                    .Where(a => roleIdList.Contains(a.RoleId) && permission == a.Name)
                    .Count() > 0;
        }

        public async Task<int> CreateOrUpdateRoleAsync(Role role)
        {
            //Check Role Name uniqueness
            if(Roles.Where(r => r.Name == role.Name && r.Id != role.Id).Count() == 0)
                return await RoleRepository.InsertOrUpdateAndGetIdAsync(role);

            return -1;
        }

        public async Task<int> DeleteRoleAsync(int roleId)
        {
            if (Roles.Where(r => r.Id == roleId).Count() == 0)
                return -1;
            else
            {
                await RoleRepository.DeleteAsync(roleId);
                return 0;
            }
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await Roles.ToListAsync();
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<int> AssignPermissionToRoleAsync(ErmesPermission permission)
        {
            if (Permissions.Where(p => p.RoleId == permission.RoleId && p.Name == permission.Name).Count() > 0)
                return -1;
            else if (Roles.Where(r => r.Id == permission.RoleId).Count() == 0)
                return -2;
            else
                return await PermissionRepository.InsertAndGetIdAsync(permission);
        }

        public async Task<int> DeletePermissionAsync(ErmesPermission permission)
        {
            var perm = await Permissions.Where(p => p.RoleId == permission.RoleId && p.Name == permission.Name).SingleOrDefaultAsync();
            if (perm == null)
                return -1;
            else
                await PermissionRepository.DeleteAsync(perm);

            return 0;
        }

        public async Task<bool> DeleteAllPermissionsAsync()
        {
            var permList = await Permissions.ToListAsync();
            foreach (var item in permList)
            {
                await PermissionRepository.DeleteAsync(item);
            }

            return true;
        }
    }
}
