using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize(Roles="admin")]
    [RouteArea("Managers")]
    [RoutePrefix("Analysis")]
    public class AnalysisController : Controller
    {
        private IRepository<Product> ProductRepo;
        private IRepository<Auction> Auctionrepo;
        private IRepository<Batch> ProductInStoreRepo;
        private IRepository<Transaction> TransactionRepo;
        private IRepository<ProductToBeAuctioned> ToBeAuctionedRepo;
        private ApplicationUserManager userManager;



        private ProfitLogic ProfitLogic;
        private ProductsLogic ProductsLogic;
        private SalesLogic SalesLogic;


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


        public AnalysisController(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo,
            IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo,
            IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo,
            IRepository<AuctionTransactionStatus> _AuctionStatusTransactionRepo, ProfitLogic _ProfitLogic,
            SalesLogic _SalesLogic, ProductsLogic _ProductsLogic)
        {
            ProductRepo = _ProductRepo;
            Auctionrepo = _AuctionRepo;
            ProductInStoreRepo = _ProductInStoreRepo;
            TransactionRepo = _TransactiontRepo;
            ToBeAuctionedRepo = _ProductToBeAuctionedRepo;

            ProfitLogic = _ProfitLogic;
            SalesLogic = _SalesLogic;
            ProductsLogic = _ProductsLogic;
        }


        public ActionResult Index(string type)
        {
            ViewBag.type = type;
            ViewBag.Products = ProductRepo.GetAll(x=>x.StoreId==LoggedInUser.StoreId).ToList().Select(x => 
            new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View();
        }

        public ActionResult ProfitForProduct(Guid id)
        {
            List<ProfitLossForBatch> BatchProfitRecords = ProfitLogic.ProfitsForProductByBatch(id);
            ProfitsForProduct YearlyProfitRecords = ProfitLogic.GetProfitForProduct(id);
            ViewBag.Product = ProductRepo.Get(x => x.Id == id);
            return View(new ProfitForProductAnalysisVM
            {
                BatchProfitRecords = BatchProfitRecords,
                YearlyProfitRecords = YearlyProfitRecords,
            });
        }

        public ActionResult TurnoverForProduct(Guid id)
        {
            TurnoverForProduct TurnoverRecords = SalesLogic.GetTurnoverForProduct(id);
            ViewBag.id = id;
            return View(TurnoverRecords);
        }

        [Route("TurnoverReportDetails/{id}/{year:int}")]
        public ActionResult TurnoverReportDetails(Guid id, int year)
        {
            int Year = year;
            TurnoverForProduct TurnoverRecords = SalesLogic.GetTurnoverForProduct(id);
            return View(TurnoverRecords.TurnoverForProductInYear.FirstOrDefault(x=>x.Year==Year));
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
                else if (ToBeAuctionedRepo.GetAll(x => x.BatchId == item.Id && x.HasBeenReviewedByManager == true) != null)
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