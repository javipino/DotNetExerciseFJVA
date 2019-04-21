using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integration.DataInfraestructure
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get();
        Task<T> GetByPK(T entity);
        T Insert(T entity);
        Task<int> Update(T entity);
        Task<int> DeletePhysical(T entity);
        Task<int> Truncate(T entity);
    }
}
