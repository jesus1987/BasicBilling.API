
using System.Linq;
using System.Threading.Tasks;

namespace BasicBilling.Service.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Query { get; }
        Task<T> Create(T model);
        Task<T> Update(T model);
        Task<T> Delete(T model);
    }
}
