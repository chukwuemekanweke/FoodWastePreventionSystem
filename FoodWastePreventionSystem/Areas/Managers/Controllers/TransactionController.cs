using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using FoodWastePreventionSystem.BusinessLogic;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize]
    [RouteArea("Managers")]
    [RoutePrefix("Transaction")]
    public class TransactionController : Controller
    {
        private IRepository<Batch> BatchRepo;
        private IRepository<Transaction> TransactionRepo;
        private IRepository<Product> ProductRepo;
        private int PageSize = 2;


        private SalesLogic SalesLogic;

        public TransactionController(IRepository<Product> _ProductRepo, IRepository<Batch> _BatchRepo,
                                  IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo,
                                  IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo,
                                  IRepository<AuctionTransactionStatus> _AuctionTransactionStausRepo)
        {
            BatchRepo = _BatchRepo;
            TransactionRepo = _TransactiontRepo;
            ProductRepo = _ProductRepo;

            SalesLogic = new SalesLogic(_ProductRepo, BatchRepo, TransactionRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStausRepo);

        }

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
                return  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }


        // GET: Managers/Transaction
        public ActionResult Index()
        {
            return View();
        }

       
        [Route("Transaction/{type}")]
        public ActionResult Transactions(TransactionType type)
        {
            List<TransactionVM> AllSalesInfo = new List<TransactionVM>();

            if (type == TransactionType.Sale) {
                List<Transaction> AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Sale).ToList();
                foreach (var item in AllSales)
                {
                    AllSalesInfo.Add(new TransactionVM()
                    {
                        Agent = UserManager.FindById(item.AgentId),
                        Transaction = item,
                    });
                }              

            }
            else if(type == TransactionType.Auction)
            {
                List<Transaction> AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Auction).ToList();
                foreach (var item in AllSales)
                {
                    AllSalesInfo.Add(new TransactionVM()
                    {
                        Auctionee = UserManager.FindById(item.AuctioneeId),
                        Transaction = item,
                    });
                }
            }

            ViewBag.TransactionType = type;
            ViewBag.Products = ProductRepo.GetAll().Select(x => new SelectListItem(){Text=x.Name, Value=x.Id .ToString()}).ToList();
            return View(AllSalesInfo);
        }

        [OutputCache(Duration = 60, VaryByParam ="id",Location = System.Web.UI.OutputCacheLocation.Client)]
        public PartialViewResult GetTransactionsForProduct(Guid? id, TransactionType type)
        {
            List<Transaction> AllSales = new List<Transaction>();
            List<TransactionVM> AllSalesInfo = new List<TransactionVM>();

            if (type == TransactionType.Sale)
            {
                if (id == null)
                {
                    AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Sale).ToList();
                }
                else
                {
                    AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Sale && x.Batch.ProductId == id).ToList();
                }
                foreach (var item in AllSales)
                {
                    AllSalesInfo.Add(new TransactionVM()
                    {
                        Agent = UserManager.FindById(item.AgentId),
                        Transaction = item,
                    });
                }
                return PartialView("_SalesTable", AllSalesInfo);
            }
            else
            {
                if (id == null)
                {
                    AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Auction).ToList();
                }
                else
                {
                    AllSales = TransactionRepo.GetAll(x => x.TransactionType == TransactionType.Auction && x.Batch.ProductId == id).ToList();
                }
                foreach (var item in AllSales)
                {
                    AllSalesInfo.Add(new TransactionVM()
                    {
                        Auctionee = UserManager.FindById(item.AuctioneeId),
                        Transaction = item,
                    });
                }
                return PartialView("AuctionTable", AllSalesInfo);
            }
        }


        public ActionResult SelectProductToAddTransaction()
        {
            ViewBag.Products = ProductRepo.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return View();
        }

        public ActionResult AddTransaction(Guid id)
        {
            Debug.WriteLine($"Get Batch Id {BatchRepo.Get(x => x.Id == id, "Transactions").Id}");
            return View(new AddTransactionVM() { Transaction = new Transaction(), Batch = BatchRepo.Get(x => x.Id == id, "Transactions") });
        }

        [HttpPost]
        public async Task<ActionResult> AddTransaction(Transaction model)
        {
            Debug.WriteLine(model.DateOfTransaction);


            Transaction Record = await SalesLogic.AddSalesTransactionAsync(model, LoggedInUser.Id);
            if (Record != null)
            {
                TempData["msg"] = "Transaction Has been Successfully Registered";
                TempData["class"] = "alert alert-success alert-dismissable";
            }

            else
            {
                TempData["msg"] = "Oops!! An Error Occured, Could Not Register Transaction ";
                TempData["class"] = "alert alert-danger alert-dismissable";
            }


            return RedirectToAction("AddTransaction", new { id = model.BatchId });
        }

    }
}