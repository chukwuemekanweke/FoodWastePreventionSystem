using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Infrastructure
{
    public class BackgroundTasks
    {

        //public IRepository<Product> ProductRepo { get; set; }
        private ProductsLogic ProductLogic {get;set;}
        private EmailLogic EmailLogic { get; set; }

        public BackgroundTasks( ProductsLogic _ProductL, EmailLogic _EmailL)
        {
            ProductLogic = _ProductL;
            EmailLogic = _EmailL;
        }

        public void SearchForExpiredproduct()
        {
            try
            {
                RecurringJob.AddOrUpdate("search-for-auction-products", ()=> ProductLogic.ProductsPast75PercentOfShelfLife(), Cron.Daily);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AddProductToAuctionTable()
        {
            try
            {
                RecurringJob.AddOrUpdate("add-product-to-auction-table", () => ProductLogic.ListProductsToBeAuctioned(), Cron.Daily);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void SendInventoryEmails()
        {
            try
            {
                RecurringJob.AddOrUpdate("send-inventory-emails", () => EmailLogic.PrepareMail(), Cron.Daily);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public void AddProductBackground(Guid storeId)
        //{
        //    try {
        //        RecurringJob.AddOrUpdate("process-notifications", () => ProductRepo.Add(new Product()
        //        {
        //            Name = "Cron Product",
        //            Description = "crooooooon",
        //            Category = "CRON",
        //            Blacklisted = false,
        //            QuantityPerCarton = 60,
        //            DateProductWasRegistered = DateTime.Now,
        //            BulkPurchaseDiscountPercent = 6,
        //            StoreId = storeId,

        //        }), Cron.Minutely);
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        throw e;
        //    }
        //}
    }
}