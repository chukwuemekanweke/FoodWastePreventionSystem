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
using PagedList;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize]
    [RouteArea("Managers")]
    [RoutePrefix("Auction")]
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

        private int PageSize = 10;

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


        [Route("ItemsOnAuction/{searchTerm}/{page:int}")]
        [Route("ItemsOnAuction/{searchTerm}")]
        [Route("ItemsOnAuction/{searchCategory}")]
        [Route("ItemsOnAuction/{page:int}")]
        [Route("ItemsOnAuction")]
        public ActionResult ItemsOnAuction(string searchTerm, string searchCategory, int? page)
        {
            var models = new List<Auction>();

            if (!string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))
            {
                models =  Auctionrepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId).Where(x => x.Batch.Product.Category == searchCategory && x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm) && string.IsNullOrWhiteSpace(searchCategory))
            {
                models =  Auctionrepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId).Where(x => x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }
            else if (string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))
            {
                models =  Auctionrepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId).Where(x => x.Batch.Product.Category == searchCategory).ToList();
            }
            else
            {
                models =  Auctionrepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId).ToList();
            }

            if (string.IsNullOrWhiteSpace(searchCategory))
            {
                ViewBag.Catgory = searchCategory;
                ViewBag.SearchTerm = searchTerm;
            }

            int pageNumber = (page ?? 1);
            ViewBag.Categories = ProductRepo.GetAll(x => x.StoreId == LoggedInUser.StoreId).Select(x => x.Category).Distinct().ToList();

            return View(models.ToPagedList(pageNumber, PageSize));
            
        }



        public ActionResult AboutToBeAuctioned(string searchTerm, string searchCategory, int? page)
        {
            var models = new List<ProductToBeAuctioned>();

            if (!string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))
            {
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager==true).Where(x => x.Batch.Product.Category == searchCategory && x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm) && string.IsNullOrWhiteSpace(searchCategory))
            {
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == true).Where(x => x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }
            else if (string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))
            {
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == true).Where(x => x.Batch.Product.Category == searchCategory).ToList();
            }
            else
            {
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == true).ToList();
            }

            if (string.IsNullOrWhiteSpace(searchCategory))
            {
                ViewBag.Catgory = searchCategory;
                ViewBag.SearchTerm = searchTerm;
            }

            int pageNumber = (page ?? 1);
            ViewBag.Categories = ProductRepo.GetAll(x => x.StoreId == LoggedInUser.StoreId).Select(x => x.Category).Distinct().ToList();

            return View(models.ToPagedList(pageNumber, PageSize));

        }

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

        [Route("ReviewPendingAuctions/{searchTerm}/{page:int}")]
        [Route("ReviewPendingAuctions/{searchTerm}")]
        [Route("ReviewPendingAuctions/{searchCategory}")]
        [Route("ReviewPendingAuctions/{page:int}")]
        [Route("ReviewPendingAuctions")]
        public ActionResult ReviewPendingAuctions(string searchTerm, string searchCategory, int? page) 
           {
            var models = new List<ProductToBeAuctioned>();

            if (!string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))
            {
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == false).Where(x => x.Batch.Product.Category == searchCategory && x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }                                                                                                                           
            else if (!string.IsNullOrWhiteSpace(searchTerm) && string.IsNullOrWhiteSpace(searchCategory))                               
            {                                                                                                                            
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == false).Where(x => x.Batch.Product.Name.ToLower().Contains(searchTerm.ToLower()) == true).ToList();
            }                                                                                                                           
            else if (string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(searchCategory))                               
            {                                                                                                                           
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == false).Where(x => x.Batch.Product.Category == searchCategory).ToList();
            }                                                                                                                           
            else                                                                                                                        
            {                                                                                                                            
                models = ToBeAuctionedRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId && x.HasBeenReviewedByManager == false).ToList();
            }

            if (string.IsNullOrWhiteSpace(searchCategory))
            {
                ViewBag.Catgory = searchCategory;
                ViewBag.SearchTerm = searchTerm;
            }

            int pageNumber = (page ?? 1);
            ViewBag.Categories = ProductRepo.GetAll(x => x.StoreId == LoggedInUser.StoreId).Select(x => x.Category).Distinct().ToList();

            return View(models.ToPagedList(pageNumber, PageSize));
        }

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