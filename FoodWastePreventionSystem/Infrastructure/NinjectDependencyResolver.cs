using FoodWastePreventionSystem.Models;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Infrastructure
{
    public class NinjectDependencyResolver:System.Web.Mvc.IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public IDependencyScope BeginScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {
            



            kernel.Bind<IRepository<Product>>().To<Repository<Product>>();
            kernel.Bind<IRepository<Store>>().To<Repository<Store>>();
            kernel.Bind<IRepository<Batch>>().To<Repository<Batch>>();
            kernel.Bind<IRepository<Transaction>>().To<Repository<Transaction>>();
            kernel.Bind<IRepository<ProductToBeAuctioned>>().To<Repository<ProductToBeAuctioned>>();
            kernel.Bind<IRepository<Loss>>().To<Repository<Loss>>();
            kernel.Bind<IRepository<Auction>>().To<Repository<Auction>>();
            kernel.Bind<IRepository<AuctionTransactionStatus>>().To<Repository<AuctionTransactionStatus>>();


        }
    }
}