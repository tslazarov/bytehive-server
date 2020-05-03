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
    public class FilesService : IFilesService
    {
        private readonly IFilesRepository filesRepository;

        public FilesService(IFilesRepository filesRepository)
        {
            this.filesRepository = filesRepository;
        }

        public async Task<TModel> GetFile<TModel>(Guid id)
        {
            return await this.filesRepository.Get<TModel>(id);
        }

        public async Task<IEnumerable<TModel>> GetFiles<TModel>()
        {
            return await this.filesRepository.GetAll<TModel>();
        }

        public async Task<bool> Create(File file)
        {
            return await this.filesRepository.Create<File>(file);
        }

        public async Task<bool> Update(File file)
        {
            return await this.filesRepository.Update<File>(file);
        }

        public async Task<bool> Delete(File file)
        {
            return await this.filesRepository.Delete(file.Id);
        }
    }
}
