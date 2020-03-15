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
    public class UsersRepository : IUsersRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public UsersRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.Users
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.Users
                .Where(u => u.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<TModel> Get<TModel>(string email)
        {
            return await this.db.Users
                .Where(u => u.Email == email)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel user)
        {
            if (user is User)
            {
                this.db.Users.Add(user as User);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel user)
        {
            if (user is User)
            {
                this.db.Entry(user).State = EntityState.Modified;
                this.db.Users.Update(user as User);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task Remove(Guid id)
        {
            var user = await this.db.Users.FindAsync(id);
            this.db.Users.Remove(user);

            await this.db.SaveChangesAsync();
        }
    }
}
