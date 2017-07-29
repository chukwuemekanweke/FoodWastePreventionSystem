using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using System.Diagnostics;
using PagedList;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize]
    [RouteArea("Managers")]
    [RoutePrefix("Batches")]
    public class BatchesController : Controller
    {
        private IRepository<Product> ProductRepo;
        private IRepository<Auction> Auctionrepo;
        private IRepository<Batch> ProductInStoreRepo;
        private IRepository<Transaction> TransactionRepo;
        private IRepository<ProductToBeAuctioned> ToBeAuctionedRepo;

        private AuctionLogic AuctionLogic;

        private ApplicationUserManager userManager;
        private ApplicationUser loggedInUser;

        private int PageSize = 5;

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


        public BatchesController(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionStatusTransactionRepo)
        {
            ProductRepo = _ProductRepo;
            Auctionrepo = _AuctionRepo;
            ProductInStoreRepo = _ProductInStoreRepo;
            TransactionRepo = _TransactiontRepo;
            ToBeAuctionedRepo = _ProductToBeAuctionedRepo;

            AuctionLogic = new AuctionLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionStatusTransactionRepo);

        }

        // GET: Managers/Batches
        public ActionResult BatchView()
        {
            ViewBag.Products = ProductRepo.GetAll().ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View();
        }
             

        public ActionResult AddBatch(Guid? id)
        {           
            ViewBag.Products = ProductRepo.GetAll().ToList().Select(x=> new SelectListItem() { Text = x.Name, Value=x.Id.ToString(), Selected=(x.Id==id)}).ToList();
            return View(new Batch());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult AddBatch(Batch model)
        {
            if (ModelState.IsValid)
            {
                ProductInStoreRepo.Add(model);
                return RedirectToAction("BatchesForProduct",new {id=model.ProductId });                
            }
            ViewBag.Products = ProductRepo.GetAll().ToList().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View(model);
        }

        [Route("BatchesForProduct/{id}/{page:int}")]
        [Route("BatchesForProduct/{id}")]
        public ActionResult BatchesForProduct(Guid id, int? page)
        {
            int pageNumber = (page ?? 1);
            var model = ProductRepo.Get(x => x.Id == id, "Batches").Batches;
            return View(model.ToPagedList(pageNumber, PageSize));
        }
        

        public ActionResult BatchInformation(Guid id)
        {
            List<TransactionVM> SalesWithAgent = new List<TransactionVM>();
            IEnumerable<TransactionVM> SalesWithoutAgent =  TransactionRepo.GetAll(x => x.BatchId == id && x.TransactionType == TransactionType.Sale).ToList().Select(x => new TransactionVM() { Transaction = x });
            IEnumerable<TransactionVM> Auctions = AuctionLogic.FetchCompletedAuctionTransactionsForBatch(id).ToList().Select(x => new TransactionVM() { Transaction = x });

            Auction AuctionInformation = Auctionrepo.Get(x => x.BatchId == id);

            foreach (var item in SalesWithoutAgent)
            {
                item.Agent = UserManager.FindById(item.Transaction.AgentId);
                SalesWithAgent.Add(item);            
            }
            foreach (var item in Auctions)
            {
                item.Auctionee = UserManager.FindById(item.Transaction.AuctioneeId);

            }
            foreach (var item in SalesWithAgent)
            {
                Debug.WriteLine(item.Agent.UserName);
                Debug.WriteLine(item.Transaction.Batch.ManufactureDate);
                Debug.WriteLine(item.Transaction.Batch.Product.Name);
            }

            BatchInformationVM Records = new BatchInformationVM()
            {
                Transactions = SalesWithAgent,
                AuctionRecords = Auctions,
                ToBeAuctionedRecord = ToBeAuctionedRepo.Get(x => x.BatchId == id),
                BatchInfo = ProductInStoreRepo.Get(x => x.Id == id),

                AuctionDetails = AuctionInformation,
            };
            ViewBag.HA = Records.BatchInfo.DateWhichInventoryWasPurchased;
            return View(Records);
        }

       
        public PartialViewResult FetchListOfBatchesForProduct(Guid id)=>       
             PartialView("_Batches", ProductRepo.Get(x => x.Id == id).Batches.ToList());
        
        
    }
}