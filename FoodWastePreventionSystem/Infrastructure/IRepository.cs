using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FoodWastePreventionSystem.Infrastructure
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null,string IncludeProperties=null);
        T Get(Expression<Func<T, bool>> predicate, string IncludeProperties = null);
        T Add(T entity);
        T Update(T entity);
        void Delete(Guid id);
        long Count();

        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, string IncludeProperties = null);
        Task DeleteAsync(Guid id);
    }


    //Generic IRepository Implementation
   public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationContext AppContext = null;
        DbSet<T> m_DbSet = null;
        public Repository(ApplicationContext context)
        {
            AppContext = context;
            m_DbSet = AppContext.Set<T>();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate =null, string includeProperties = null)
        {
            IQueryable<T> query = m_DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if(predicate!=null)
            query = query.Where(predicate);

            return query;
        }

        public T Get(Expression<Func<T, bool>> predicate,string includeProperties)
        {
            IQueryable<T> query = m_DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {               
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Debug.WriteLine(includeProperty);
                    query = query.Include(includeProperty);
                }
            }
            if (predicate != null)
                return query.FirstOrDefault(predicate);
            else
                return null;
            
        }

        public T Add(T entity)
        {
            T NewEntity = m_DbSet.Add(entity);
            AppContext.SaveChanges();
            return NewEntity;

        }

        public T Update(T entity)
        {
            m_DbSet.Attach(entity);
            ((IObjectContextAdapter)AppContext).ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            AppContext.SaveChanges();

            return entity;
        }

        public void Delete(Guid id)
        {
            T Entity = m_DbSet.Find(id);
            m_DbSet.Remove(Entity);
            AppContext.SaveChanges();

        }

        public long Count()
        {
            return m_DbSet.Count();
        }

        public async Task<T> AddAsync(T entity)
        {
            T NewEntity = m_DbSet.Add(entity);
            await AppContext.SaveChangesAsync();
            return NewEntity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var t = m_DbSet.Attach(entity);
            ((IObjectContextAdapter)AppContext).ObjectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
            await AppContext.SaveChangesAsync();

            return t;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate,string includeProperties)
        {
            IQueryable<T> query = m_DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (predicate != null)
                return await query.FirstOrDefaultAsync(predicate);
            else
                return null;
        }

        public async Task DeleteAsync(Guid id)
        {
            T Entity =await  m_DbSet.FindAsync(id);
            m_DbSet.Remove(Entity);
            await AppContext.SaveChangesAsync();
        }

            

       
    }
}
