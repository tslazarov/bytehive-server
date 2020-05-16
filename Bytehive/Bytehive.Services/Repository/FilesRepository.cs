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
    public class FilesRepository : IFilesRepository
    {
        private readonly BytehiveDbContext db;
        private readonly IMapper mapper;

        public FilesRepository(BytehiveDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TModel>> GetAll<TModel>()
        {
            return await this.db.Files
                  .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                  .ToListAsync();
        }

        public async Task<TModel> Get<TModel>(Guid id)
        {
            return await this.db.Files
                .Where(sr => sr.Id == id)
                .ProjectTo<TModel>(this.mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create<TModel>(TModel file)
        {
            if (file is File)
            {
                this.db.Files.Add(file as File);

                await this.db.SaveChangesAsync();


                return true;
            }

            return false;
        }

        public async Task<bool> Update<TModel>(TModel file)
        {
            if (file is File)
            {
                var localFile = file as File;
                this.db.DetachLocal(localFile, localFile.Id);
                this.db.Files.Update(localFile);
                await this.db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<bool> Delete(Guid id)
        {
            var file = await this.db.Files.FindAsync(id);

            if (file != null)
            {
                this.db.Files.Remove(file);
                await this.db.SaveChangesAsync();

                return true;

            }

            return false;
        }
    }
}
