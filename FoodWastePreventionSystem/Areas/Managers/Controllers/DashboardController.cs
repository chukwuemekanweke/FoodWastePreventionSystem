using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    public class DashboardController : Controller
    {
        private ProfitLogic ProfitLogic { get; set; }
        private SalesLogic SalesL { get; set; }

        private IRepository<Product> ProductRepo { get; set; }
        private IRepository<Transaction> TransactionRepo { get; set; }


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
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

        }


        public DashboardController(IRepository<Product> _ProductRepo, IRepository<Transaction> _transaction, SalesLogic _SalesL, ProfitLogic _ProfitL, ChartLogic _ChartL)
        {
            ProductRepo = _ProductRepo;
            TransactionRepo = _transaction;
            SalesL = _SalesL;
            ProfitLogic = _ProfitL;
        }
        public ActionResult Index()
        {
            ViewBag.profits = ProfitLogic.GetProfitForProducts(x => x.StoreId == LoggedInUser.StoreId).Sum(x => x.ProfitsForProductInYear.Sum(y => y.ProfitsPerMonth.Sum(z=>z.Value)));
            ViewBag.sales = SalesL.GetSalesForProducts(x => x.StoreId == LoggedInUser.StoreId).Sum(x => x.TransactionsForProductInYear.Sum(y => y.TransactionsPerMonth.Sum(z => z.Value)));
            ViewBag.transactions = TransactionRepo.GetAll(x => x.Batch.Product.StoreId == LoggedInUser.StoreId).Count();
            ViewBag.products = ProductRepo.GetAll(x => x.StoreId == LoggedInUser.StoreId).Count();

            ProfitLogic.GetProfitForProducts(x => x.StoreId == LoggedInUser.StoreId);
          
            return View();
        }
    }
}