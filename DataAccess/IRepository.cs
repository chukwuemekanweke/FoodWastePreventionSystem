using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using FoodWastePreventionSystem.Infrastructure;

namespace DataAccess
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null);
        T Get(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        long Count();
    }









    class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationContext AppContext = null;
        DbSet<T> m_DbSet;
        public Repository(ApplicationContext context)
        {
            AppContext = context;
            m_DbSet = AppContext.Set<T>();
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                return m_DbSet.Where(predicate);
            }
            return m_DbSet.AsEnumerable();
        }
        public T Get(Expression<Func<T, bool>> predicate)
        {
            return m_DbSet.FirstOrDefault(predicate);
        }
        public void Add(T entity)
        {
            m_DbSet.Add(entity);
        }
        public void Update(T entity)
        {
            m_DbSet.Attach(entity);
            ((IObjectContextAdapter)AppContext).ObjectContext.
            ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        }
        public void Delete(T entity)
        {
            m_DbSet.Remove(entity);
        }
        public long Count()
        {
            return m_DbSet.Count();
        }
    }
}
