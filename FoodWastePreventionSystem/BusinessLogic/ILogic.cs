using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class Logic
    {
        //private IRepository<Product> _ProductRepo;
        //private IRepository<ProductInStore> _ProductInStoreRepo;
        //private IRepository<Transaction> _TransactiontRepo;
        //private IRepository<Auction> _AuctionRepo;
        //private IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo;

        public IRepository<Transaction> TransactionRepo { get; set; }
        public IRepository<Product> ProductRepo { get; set; }
        public IRepository<Auction> AuctionRepo { get; set; }
        public IRepository<ProductToBeAuctioned> ProductToBeAuctionedRepo { get; set; }
        public IRepository<Batch> BatchRepo { get; set; }
        public IRepository<Loss> LossRepo { get; set; }
        public IRepository<AuctionTransactionStatus> AuctionTransactionStatusRepo { get; set; }


        public Logic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo)

        {
            ProductRepo = _ProductRepo;
            BatchRepo = _ProductInStoreRepo;
            TransactionRepo = _TransactiontRepo;
            AuctionRepo = _AuctionRepo;
            ProductToBeAuctionedRepo = _ProductToBeAuctionedRepo;
            LossRepo = _LossRepo;
            AuctionTransactionStatusRepo = _AuctionTransactionStatusRepo;

        }

       
    }
}