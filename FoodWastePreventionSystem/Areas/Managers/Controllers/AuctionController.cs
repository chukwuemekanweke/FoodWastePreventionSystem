using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    public class AuctionController : Controller
    {
        private IRepository<Product> ProductRepo;
        private IRepository<Auction> Auctionrepo;
        private IRepository<Batch> ProductInStoreRepo;
        private IRepository<Transaction> TransactionRepo;
        private IRepository<ProductToBeAuctioned> ToBeAuctionedRepo;

        private ProductsLogic ProductsLogic;
        private Guid itemid;

        private ApplicationUserManager userManager;
        private ApplicationUser loggedInUser;

        public ApplicationUser LoggedInUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }



        public ApplicationUserManager UserManager
        {
            get
            {
                return userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }


        public AuctionController(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionStatusTransactionRepo)
        {
            ProductRepo = _ProductRepo;
            Auctionrepo = _AuctionRepo;
            ProductInStoreRepo = _ProductInStoreRepo;
            TransactionRepo = _TransactiontRepo;
            ToBeAuctionedRepo = _ProductToBeAuctionedRepo;

            ProductsLogic = new ProductsLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionStatusTransactionRepo);

        }

        public ActionResult ItemsOnAuction() =>
            View(Auctionrepo
                .GetAll(x => x.Batch.QuantityPurchased > (x.Batch.QuantitySold + x.Batch.QuantityDisposedOf + x.Batch.QuantityAuctioned))
                .ToList()
                );



        public ActionResult AboutToBeAuctioned() =>
            View(
                ToBeAuctionedRepo.GetAll(x => x.HasBeenReviewedByManager == true).ToList()
                );

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AboutToBeAuctioned(ProductToBeAuctioned model)
        {

            var ReturnUrl = RouteData.Values["returnUrl"];
            ProductToBeAuctioned Record = ToBeAuctionedRepo.Get(x => x.Id == model.Id);
            Record.AuctionPrice = model.AuctionPrice;
            Record.DateOfAuction = model.DateOfAuction;
            Record = ToBeAuctionedRepo.Update(Record);
            TempData["class"] = "alert alert-success alert-dismissable";
            TempData["message"] = "CHanges Saved Successfully";

            return RedirectToAction("AboutToBeAuctioned");
        }


        public ActionResult ReviewPendingAuctions() =>
            View(
                    ToBeAuctionedRepo.GetAll(x=>x.HasBeenReviewedByManager==false).ToList()
                );

        [HttpPost]
        public ActionResult ApproveAuctionProposal(Guid id)
        {
            ProductToBeAuctioned Record = ToBeAuctionedRepo.Get(x => x.Id == id);
            Record.HasBeenReviewedByManager = true;
            ToBeAuctionedRepo.Update(Record);

            TempData["class"] = "alert alert-success alert-dismissable";
            TempData["message"] = $"Auction Proposal For {Record.Batch.Product.Name} Has Been Accepted";
            return RedirectToAction("ReviewPendingAuctions");
        }

        [HttpPost]
        public ActionResult RejectAuctionProposal(Guid id)
        {
            ProductToBeAuctioned Record = ToBeAuctionedRepo.Get(x => x.Id == id);            
            ToBeAuctionedRepo.Delete(id);
            TempData["class"] = "alert alert-warning alert-dismissable";
            TempData["message"] = $"Auction Proposal For {Record.Batch.Product.Name} Has Been Rejected";
            return RedirectToAction("ReviewPendingAuctions");
        }


        [HttpPost]
        public ActionResult EditAuctionPrice(Auction model)
        {
            Auction Record = Auctionrepo.Get(x => x.Id == model.Id);
            Record.AuctionPrice = model.AuctionPrice;
            Auctionrepo.Update(Record);
            TempData["class"] = "alert alert-success alert-dismissable";
            TempData["message"] = "CHanges Saved Successfully";
            return RedirectToAction("ItemsOnAuction");
        }



        public ActionResult EditAboutToBeAuctionedRecord(Guid id)
        {
            Guid BatchId = id;
            ProductToBeAuctioned Record = ToBeAuctionedRepo.Get(x => x.BatchId == BatchId);
            if (Record == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(Record);
        }



       


        public ActionResult ViewAuction(Guid id)
        {
            Guid BatchId = id;
            List<Transaction> AuctionTransactionRecords = new List<Transaction>();
            List<TransactionVM> AuctionTransactionFullRecords = new List<TransactionVM>();

            Auction AuctionRecord = Auctionrepo.Get(x => x.BatchId == BatchId);
            if (AuctionRecord == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            AuctionTransactionRecords = AuctionRecord.Batch.Transactions.Where(x => x.TransactionType == TransactionType.Auction).ToList();
            AuctionTransactionFullRecords = AuctionTransactionRecords.Select(x => new TransactionVM()
            {
                Transaction = x,
                Agent = UserManager.FindById(x.AuctioneeId),
            }).ToList();

            return View(new EditViewAuctionVM()
            {
                Auction = AuctionRecord,
                TransactionRecords = AuctionTransactionFullRecords,
            });
        }

        [HttpPost]
        public ActionResult EditAuction(Auction model)
        {
            Auction Record = Auctionrepo.Get(x => x.Id == model.Id);
            Record.AuctionPrice = model.AuctionPrice;
            Auctionrepo.Update(Record);
            return RedirectToAction("ViewSoonToBeExpiredInventory", "Analysis");
        }

    }
}