using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Repositories
{
    public class DataRepository<TModel> : IDataRepository<TModel> where TModel : RestModel
    {
        public DataRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public void Add(TModel model)
        {
            GetDbSet().Add(model);
            DbContext.SaveChanges();
        }

        public async Task AddAsync(TModel model)
        {
            await GetDbSet().AddAsync(model);
            await DbContext.SaveChangesAsync();
        }

        public void Delete(TModel model)
        {
            GetDbSet().Remove(model);
            DbContext.SaveChanges();
        }

        public Task DeleteAsync(TModel model)
        {
            return Task.Run(() => Delete(model));
        }

        public TModel Get(int id)
        {
            return GetDbSet().SingleOrDefault(m => m.Id == id);
        }

        public IEnumerable<TModel> GetAll()
        {
            return GetDbSet();
        }

        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await GetDbSet().ToListAsync();
        }

        public Task<TModel> GetAsync(int id)
        {
            return GetDbSet().SingleOrDefaultAsync(m => m.Id == id);
        }

        public void Update(TModel model)
        {
            GetDbSet().Update(model);
            DbContext.SaveChanges();
        }

        public Task UpdateAsync(TModel model)
        {
            return Task.Run(() => Update(model));
        }

        private DbSet<TModel> GetDbSet() => DbContext.Set<TModel>();

        public DbContext DbContext { get; }
    }
}