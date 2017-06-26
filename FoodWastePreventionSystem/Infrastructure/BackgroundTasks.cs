using FoodWastePreventionSystem.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Infrastructure
{
    public  class BackgroundTasks
    {

        public  IRepository<Product> ProductRepo { get; set; }       

        public BackgroundTasks(IRepository<Product> _ProductRepo)
        {
            ProductRepo = _ProductRepo;
        }
        public void AddProductBackground(Guid storeId)
        {
            try {
                RecurringJob.AddOrUpdate("process-notifications", () => ProductRepo.Add(new Product()
                {
                    Name = "Cron Product",
                    Description = "crooooooon",
                    Category = "CRON",
                    Blacklisted = false,
                    QuantityPerCarton = 60,
                    DateProductWasRegistered = DateTime.Now,
                    BulkPurchaseDiscountPercent = 6,
                    StoreId = storeId,
                    
                }), Cron.Minutely);
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
        }
    }
}