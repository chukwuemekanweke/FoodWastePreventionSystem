using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    public class AnalysisController : Controller
    {
        private IRepository<Product> ProductRepo;
        private IRepository<Auction> Auctionrepo;
        private IRepository<Batch> ProductInStoreRepo;
        private IRepository<Transaction> TransactionRepo;
        private IRepository<ProductToBeAuctioned> ToBeAuctionedRepo;

        private ProductsLogic ProductsLogic;
        private Guid itemid;

        public AnalysisController(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionStatusTransactionRepo)
        {
            ProductRepo = _ProductRepo;
            Auctionrepo = _AuctionRepo;
            ProductInStoreRepo = _ProductInStoreRepo;
            TransactionRepo = _TransactiontRepo;
            ToBeAuctionedRepo = _ProductToBeAuctionedRepo;

            ProductsLogic = new ProductsLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionStatusTransactionRepo);

        }
        // GET: Managers/Analysis
        public ActionResult ViewSoonToBeExpiredInventory()
        {
            List<Batch> ProductsPast75PercentOfShelfLife = ProductsLogic.ProductsPast75PercentOfShelfLife().ToList();
            List<AuctionManagementVM> BatchAuctionRecords = new List<AuctionManagementVM>();
            foreach (var item in ProductsPast75PercentOfShelfLife)
            {
                if (Auctionrepo.GetAll(x => x.BatchId == item.Id) != null)
                {
                    BatchAuctionRecords.Add(new AuctionManagementVM() { Batch = item, State = BatchAuctionState.OnAuction });
                }
                else if (ToBeAuctionedRepo.GetAll(x => x.BatchId == item.Id&& x.HasBeenReviewedByManager==true) != null)
                {
                    BatchAuctionRecords.Add(new AuctionManagementVM() { Batch = item, State = BatchAuctionState.AboutToBeAuctioned });
                }
                else if (ToBeAuctionedRepo.GetAll(x => x.BatchId == item.Id && x.HasBeenReviewedByManager == false) != null)
                {
                    BatchAuctionRecords.Add(new AuctionManagementVM() { Batch = item, State = BatchAuctionState.NotReviewed });
                }
            }
            return View(BatchAuctionRecords);
        }
    }
}