using System.Collections.Generic;
using System.Threading.Tasks;
using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Services.Repositories
{
    public interface IDataRepository<TModel> where TModel : RestModel
    {
        IEnumerable<TModel> GetAll();
        Task<IEnumerable<TModel>> GetAllAsync();
        TModel Get(int id);
        Task<TModel> GetAsync(int id);
        void Add(TModel model);
        Task AddAsync(TModel model);
        void Update(TModel model);
        Task UpdateAsync(TModel model);
        void Delete(TModel model);
        Task DeleteAsync(TModel model);
    }
}