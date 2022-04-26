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

        public T Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _table;

            if (includes != null)
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));

            return query.Where(criteria).FirstOrDefault();
        }

        public IQueryable<T> GetAll() => _table;

        public T GetById(int id) => _table.Find(id);

        public void Add(T entity) => _table.Add(entity);

        public void Remove(T entity) => _table.Remove(entity);

        public void RemoveById(int id) => _table.Remove(GetById(id));

        public void Update(T entity) => _context.Update(entity);
    }
}
