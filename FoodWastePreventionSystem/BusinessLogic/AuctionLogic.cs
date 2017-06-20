using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class AuctionLogic : Logic
    {

        public AuctionLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStausRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStausRepo)
        {
        }

        public List<Batch> ViewProductsOnAuction()
        {
            List<Auction> ActiveAuction = AuctionRepo.GetAll(x => (x.Batch.QuantitySold + x.Batch.QuantityAuctioned) < x.Batch.QuantityPurchased).ToList();
            List<Batch> ProductOnAuctionRecords = ActiveAuction.Select(x => x.Batch).ToList();
            return ProductOnAuctionRecords;
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




    }
}