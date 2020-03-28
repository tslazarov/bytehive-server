﻿using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IRefreshTokenRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid tokenId);

        Task<bool> Create<TModel>(TModel token);

        Task<bool> Update<TModel>(TModel token);

        Task Remove(Guid id);
    }
}