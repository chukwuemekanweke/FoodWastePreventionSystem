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
    public class TransactionLogic : Logic
    {

        public TransactionLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStausRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStausRepo)
        {

        }

        public List<Transaction> ViewAuctionTransactionforProductBatch(Guid productInStoreId) =>
            ViewQueryableAuctionTransactionforProductBatch(productInStoreId).ToList();

        public IQueryable<Transaction> ViewQueryableAuctionTransactionforProductBatch(Guid productInStoreId) =>
            TransactionRepo.GetAll(x => x.BatchId == productInStoreId && x.TransactionType == TransactionType.Auction);

        public int QuantityOfProductSoldThroughAuction(Guid ProductInStoreId, Expression<Func<Transaction, bool>> TransactionPredicate = null) =>
            ViewQueryableAuctionTransactionforProductBatch(ProductInStoreId).Where(TransactionPredicate).Sum(x => x.Quantity);

        public List<Transaction> GetTransactionsForProduct(Guid productId)
        {
            return TransactionRepo.GetAll(x => x.Batch.ProductId == productId).ToList();

        }


    }
}