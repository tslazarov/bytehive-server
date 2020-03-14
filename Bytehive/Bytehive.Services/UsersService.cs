using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bytehive.Data;
using Bytehive.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive.Services
{
    public class UsersService : IUsersService
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public UsersService(BytehiveDbContext db,
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

        public async Task Remove(Guid id)
        {
            var user = await this.db.Users.FindAsync(id);
            this.db.Users.Remove(user);

            await this.db.SaveChangesAsync();
        }
    }
}
