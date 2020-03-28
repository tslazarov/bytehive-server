using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data;
using Bytehive.Data.Models;
using Bytehive.Services.Contracts;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Services.Repository
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public UserRolesRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<TModel> Get<TModel>(Guid roleId, Guid userId)
        {
            return await this.db.UserRoles
                  .Where(u => u.UserId == userId)
                  .Where(u => u.RoleId == roleId)
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel userRole)
        {
            if (userRole is UserRole)
            {
                this.db.UserRoles.Add(userRole as UserRole);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task Remove(Guid roleId, Guid userId)
        {
            var role = await this.db.UserRoles.FirstOrDefaultAsync(ur => ur.RoleId == roleId && ur.UserId == userId);

            if(role != null)
            {
                this.db.UserRoles.Remove(role);
                await this.db.SaveChangesAsync();
            }
        }
    }
}
