using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SqlServerDBContext;

namespace Repository.GenericRepository
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        #region Inject Sql Context dependency object in dependent class "Generic Repository"
        protected SqlContext _context;
        private readonly DbSet<T> _table;

        public GenericRepo(SqlContext sqlContext)
        {
            _context = sqlContext;
            _table = sqlContext.Set<T>();
        }
        #endregion

        public IQueryable<T> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _table;

            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return query.Where(criteria);
        }

        public async Task<T> Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _table;

            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return await query.Where(criteria).FirstOrDefaultAsync();
        }

        public IQueryable<T> GetAll() => _table;

        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _table;

            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return query;
        }

        public Task<T> GetById(string id) =>  Task.FromResult(_table.Find(id));

        public Task<T> GetById(int id) => Task.FromResult(_table.Find(id));

        public Task  Add(T entity) => Task.FromResult(_table.Add(entity));

        public Task  Remove(T entity) => Task.FromResult(_table.Remove(entity));

        public async Task RemoveById(string id) => _table.Remove( await GetById(id));

        public Task  Update(T entity) => Task.FromResult(_context.Update(entity));
    }
}
