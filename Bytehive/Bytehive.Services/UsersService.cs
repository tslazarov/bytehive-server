using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IRolesRepository rolesRepository;
        private readonly IUserRolesRepository userRolesRepository;

        public UsersService(IUsersRepository usersRepository,
            IRolesRepository rolesRepository,
            IUserRolesRepository userRolesRepository)
        {
            this.usersRepository = usersRepository;
            this.rolesRepository = rolesRepository;
            this.userRolesRepository = userRolesRepository;
        }
        
        public async Task<User> GetUser(Guid id)
        {
            return await this.usersRepository.Get<User>(id);
        }

        public async Task<User> GetUser(string email, string providerName)
        {
            return await this.usersRepository.Get<User>(email, providerName);
        }

        public async Task<IEnumerable<TModel>> GetUsers<TModel>()
        {
            return await this.usersRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(User user)
        {
            return await this.usersRepository.Create<User>(user);
        }

        public async Task<bool> Update(User user)
        {
            return await this.usersRepository.Update<User>(user);
        }

        public async Task<bool> Delete(User user)
        {
            return await this.usersRepository.Delete(user.Id);
        }

        public async Task<bool> AssignRole(Guid userId, string roleName)
        {
            var role = await this.rolesRepository.Get<Role>(roleName);

            if (role != null)
            {
                var userRole = await this.userRolesRepository.Get<UserRole>(role.Id, userId);
                if (userRole == null)
                {
                    userRole = new UserRole() { RoleId = role.Id, UserId = userId };
                    return await this.userRolesRepository.Create(userRole);
                }

                return true;
            }

            return false;
        }
    }
}
