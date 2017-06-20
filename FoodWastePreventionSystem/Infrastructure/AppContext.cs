using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Infrastructure
{
    public partial class ApplicationContext : DbContext
    {

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<ProductToBeAuctioned> ProductsToBeAuctioned  { get; set; }
        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<Loss> Losses { get; set; }
        public virtual DbSet<AuctionTransactionStatus> AuctionTransactionStatus { get; set; }


        public ApplicationContext(): base("name=AppConnectionString")
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }
    }

   
}