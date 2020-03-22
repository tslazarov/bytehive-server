﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Repository
{
    public interface IRolesRepository
    {
        Task<IEnumerable<TModel>> GetAll<TModel>();
 
        Task<TModel> Get<TModel>(Guid userId);

        Task<bool> Create<TModel>(TModel role);

        Task<bool> Update<TModel>(TModel role);

        Task Remove(Guid id);
    }
}
