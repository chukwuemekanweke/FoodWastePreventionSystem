using FoodWastePreventionSystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Infrastructure
{
    public class UnitOfWork<T> : IDisposable where T : class
    {

        private ApplicationContext AppContext = null;
        private Repository<T> repository = null;


        public UnitOfWork()
        {
            AppContext = new ApplicationContext();
        }

        public void SaveChanges()
        {
            AppContext.SaveChanges();
        }

        public Repository<T> Repository
        {
            get
            {
                if (Repository == null)
                {
                    repository = new Repository<T>(AppContext);
                }
                return repository;
            }
        }

        public void Dispose()
        {
            ((IDisposable)AppContext).Dispose();
        }


    }
}