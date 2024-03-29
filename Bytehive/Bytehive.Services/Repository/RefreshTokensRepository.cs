﻿using AutoMapper;
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
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public RefreshTokensRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.RefreshTokens
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(string tokenValue)
        {
            return await this.db.RefreshTokens
                .Where(u => u.Token == tokenValue)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel refreshToken)
        {
            if (refreshToken is RefreshToken)
            {
                this.db.RefreshTokens.Add(refreshToken as RefreshToken);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel refreshToken)
        {
            if (refreshToken is RefreshToken)
            {
                var localRefreshToken = refreshToken as RefreshToken;
                this.db.DetachLocal(localRefreshToken, localRefreshToken.Id);
                this.db.RefreshTokens.Update(localRefreshToken);

                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task Delete(Guid id)
        {
            var refreshToken = await this.db.RefreshTokens.FindAsync(id);
            this.db.RefreshTokens.Remove(refreshToken);

            await this.db.SaveChangesAsync();
        }
    }
}
