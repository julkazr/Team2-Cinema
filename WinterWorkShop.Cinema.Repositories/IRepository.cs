using System.Collections.Generic;
using System.Threading.Tasks;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Delete(object id);

        Task<IEnumerable<T>> GetAll();

        Task<T> GetByIdAsync(object id);

        T Insert(T obj);

        void Save();

        T Update(T obj);
    }
}
