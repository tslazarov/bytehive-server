using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bytehive.Services.Contracts.Services
{
    public interface IFilesService
    {
        Task<TModel> GetFile<TModel>(Guid id);

        Task<IEnumerable<TModel>> GetFiles<TModel>();

        Task<bool> Create(File file);

        Task<bool> Update(File file);

        Task<bool> Delete(File file);
    }
}