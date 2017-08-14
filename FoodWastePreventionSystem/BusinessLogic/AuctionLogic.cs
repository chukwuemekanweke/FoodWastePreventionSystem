using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class AuctionLogic : Logic
    {
        private SalesLogic SalesLogic;
        public AuctionLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, 
            IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo,
            IRepository<AuctionTransactionStatus> _AuctionTransactionStausRepo, SalesLogic _SalesL) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStausRepo)
        {
            SalesLogic = _SalesL;
        }

        public List<OnAuctionVM> ViewProductsOnAuction()
        {
            OnAuctionVM AuctionRecords = new OnAuctionVM();
            List<Auction> ActiveAuction = AuctionRepo.GetAll(x => (x.Batch.QuantitySold + x.Batch.QuantityAuctioned) < x.Batch.QuantityPurchased).ToList();
            List<OnAuctionVM> FullAuctionReport = new List<OnAuctionVM>();
            foreach (var item in ActiveAuction)
            {
                FullAuctionReport.Add(new OnAuctionVM() {
                    Auction = item,
                    Batch = item.Batch,
                    Product = item.Batch.Product
                });

            }

            return FullAuctionReport;
        }       

        public ProductToBeAuctioned EditAuctionDateAndPrice(Guid productToBeAuctionId, double price, DateTime auctionDate)
        {
            ProductToBeAuctioned Record = ProductToBeAuctionedRepo.Get(x => x.Id == productToBeAuctionId);
            Record.DateOfAuction = auctionDate;
            Record.AuctionPrice = price;
            return ProductToBeAuctionedRepo.Update(Record);
        }      

       

        public ProductToBeAuctioned UpdateToBeAuctionedRecord(Guid id,  DateTime dateOfAuction, double price)
        {
            ProductToBeAuctioned Record =  ProductToBeAuctionedRepo.Get(x => x.Id == id);
            Record.DateOfAuction = dateOfAuction;
            Record.AuctionPrice = price;
           return  ProductToBeAuctionedRepo.Update(Record);
        }

        public List<Transaction> FetchCompletedAuctionTransactionsForBatch(Guid productInStoreId)
        {
            IEnumerable < Transaction> AllRecords = TransactionRepo.GetAll(x => x.BatchId == productInStoreId && x.TransactionType == TransactionType.Auction);
            List<Transaction> CompletedTransactions = new List<Transaction>();
            foreach (var item in AllRecords)
            {
               AuctionTransactionStatus StateRecord =  AuctionTransactionStatusRepo.Get(x => x.TransactionId == item.Id);
                if (StateRecord.Status == AuctionState.Compleete)
                {
                    CompletedTransactions.Add(item);
                }
            }
            return CompletedTransactions;
        }


        public TransactionsForProduct GetAuctionsForProduct(Guid productId)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverPerYear = new Dictionary<int, Dictionary<Month, double>>();
            return SalesLogic.GetTransactionsForProduct(productId, out TurnoverPerYear, x => x.TransactionType == TransactionType.Auction);
        }

        public List<TransactionsForProduct> GetAuctionsForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            return SalesLogic.GetTransactionsForProducts(ProductPredicate, x => x.TransactionType == TransactionType.Auction);
        }




    }
}