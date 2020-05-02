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
    public class RolesRepository : IRolesRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public RolesRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.Roles
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.Roles
                .Where(u => u.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<TModel> Get<TModel>(string name)
        {
            return await this.db.Roles
                .Where(u => u.Name == name)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel role)
        {
            if (role is Role)
            {
                this.db.Roles.Add(role as Role);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel role)
        {
            if (role is Role)
            {
                var localRole = role as Role;
                this.db.DetachLocal(localRole, localRole.Id);
                this.db.Roles.Update(localRole);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task Delete(Guid id)
        {
            var role = await this.db.Roles.FindAsync(id);
            this.db.Roles.Remove(role);

            await this.db.SaveChangesAsync();
        }
    }
}
