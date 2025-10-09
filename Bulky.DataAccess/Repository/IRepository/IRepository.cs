using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category

        // Retrieve All Record
        IEnumerable<T> GetAll(string? includeProperties = null);

        // Retrieve Individual Record
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);

        // Delete Multiple Entity In A Single Call
        void RemoveRange(IEnumerable<T> entities);


    }
}
