using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.GenericRepository
{
    public interface IGenericRepo<T>
    {
        // Get all Entites => (get all customers, get all products ...etc)
        IQueryable<T> GetAll();

        IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes);

        // Get Entity by his ID => (get a customer by his ID)
        Task<T> GetById(string id);

        Task<T> GetById(int id);

        // Get Entity by matching given expression
        Task<T> Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        IQueryable<T> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        // Add any Entity => (add customer, add seller ...et)
        Task Add(T entity);

        // Remove an Entity => (remove a customer, remove a seller ...etc)
        Task Remove(T entity);

        Task RemoveById(string id);

        // Update Entity info => (update Customer's Info ...etc)
        Task Update(T entity);
    }
}
