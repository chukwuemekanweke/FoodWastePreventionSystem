using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize(Roles = "admin")]
    [RouteArea("Managers")]
    [RoutePrefix("Charts")]
    public class ChartsController : Controller
    {

        

        public ActionResult Index1()
        {
            return View();
        }






        private IRepository<Product> ProductRepo;
        private IRepository<Transaction> TransactionRepo;

        private SalesLogic SalesLogic { get; set; }
        private ProfitLogic ProfitLogic { get; set; }
        private ChartLogic ChartLogic { get; set; }



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


        public ChartsController(IRepository<Product> _ProductRepo, IRepository<Transaction> _TransactionRepo,  SalesLogic _SalesL, ProfitLogic _ProfitL, ChartLogic _ChartL)
        {
            ProductRepo = _ProductRepo;
            TransactionRepo = _TransactionRepo;
            SalesLogic = _SalesL;
            ProfitLogic = _ProfitL;
            ChartLogic = _ChartL;
        }

        [Route("Index")]
        [Route("")]
        public ActionResult Index()
        {            
            return View();
        }




        [HttpGet]
        public ActionResult Chart(string id)
        {
            ViewBag.operation = id;
            List<string> Categories = new List<string>();
            List<SelectListItem> CategoriesDropdown = new List<SelectListItem>();
            List<SelectListItem> ProductsDropdown = new List<SelectListItem>();

            List<Product> Products = ProductRepo.GetAll(x => x.StoreId == LoggedInUser.StoreId).ToList();


            Products.ForEach(x =>
            {
                ProductsDropdown.Add(new SelectListItem() {
                    Text = x.Name, Value = x.Id.ToString()
                });
                if (!Categories.Contains(x.Category.ToLower()))
                {
                    Categories.Add(x.Category.ToLower());
                }
            });
            foreach (var item in Categories)
            {
                CategoriesDropdown.Add(new SelectListItem() { Value = item.ToLower(), Text = item.ToUpper() });

            }
            ViewBag.categories = CategoriesDropdown;
            ViewBag.products = ProductsDropdown;

            return View();
        }

        [HttpPost]
        public ActionResult DrawChart(FormCollection collection)
        {
           string type =  collection["type"];
           string operation = collection["operation"];

            switch (type)
            {
                case "single":
                    ViewBag.productId = collection["id"];
                    Guid productId = Guid.Parse(collection["id"]);
                    ViewBag.operation = operation;
                    ViewBag.productName = ProductRepo.Get(x => x.Id == productId).Name;
                    return View("SingleProductChart");
                case "category":
                    ViewBag.category = collection["id"];
                    ViewBag.operation = operation;
                    return View("CategoryChart");
                case "all":
                    ViewBag.operation = operation;
                    return View("AllChart");
            }
            return null;

        }

        [HttpGet]
        [Route("DrawChartForProduct/{id}/{operation}")]
        public JsonResult DrawChartForProduct(string id, string operation)
        {

            Guid ProductId = Guid.Parse(id);
            Dictionary<string, double> Record = new Dictionary<string, double>();
            switch (operation)
            {
                case "profit":
                    Record = ChartLogic.ProfitForProduct(ProductId);
                    break;

                case "sale":
                    Record = ChartLogic.SalesForProduct(ProductId);
                    break;

                case "auction":
                    Record = ChartLogic.AuctionsForProduct(ProductId);
                    break;
            }

            return Json(Record, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("DrawChartForCategory/{operation}/{category}")]
        public JsonResult DrawChartForCategory(string operation, string category)
        {
           
            Dictionary<string, Dictionary<string, double>> Record = new Dictionary<string, Dictionary<string, double>>();
            switch (operation)
            {
                case "profit":
                    Record = ChartLogic.ProfitForProducts(x => x.Category == category&& x.StoreId == LoggedInUser.StoreId);
                    break;

                case "sale":
                    Record = ChartLogic.SalesForProducts(x => x.Category == category && x.StoreId == LoggedInUser.StoreId);
                    break;

                case "auction":
                    Record = ChartLogic.AuctionsForProducts(x => x.Category == category && x.StoreId == LoggedInUser.StoreId);
                    break;
            }
            return Json(Record, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("DrawChartForAllProducts/{operation}")]
        public JsonResult DrawChartForAllProducts(string operation)
        {

            Dictionary<string, Dictionary<string, double>> Record = new Dictionary<string, Dictionary<string, double>>();
            switch (operation)
            {
                case "profit":
                    Record = ChartLogic.ProfitForProducts(x=>x.StoreId==LoggedInUser.StoreId);
                    break;

                case "sale":
                    Record = ChartLogic.SalesForProducts(x => x.StoreId == LoggedInUser.StoreId);
                    break;

                case "auction":
                    Record = ChartLogic.AuctionsForProducts(x => x.StoreId == LoggedInUser.StoreId);
                    break;
            }
            ViewBag.operation = operation;
            return Json(Record, JsonRequestBehavior.AllowGet);
        }
       

        public ActionResult ProductChart()
        {
            return View();
        }

        public PartialViewResult PrepareTransactionChart(Guid id)
        {
            List<ChartsModel> quantityRecords = new List<ChartsModel>();
            List<ChartsModel> amountRecords = new List<ChartsModel>();

            Product product = ProductRepo.Get(x => x.Id == id);
            Transaction[] transactions = TransactionRepo.GetAll(x => x.Batch.ProductId == id).ToArray();
            int[] years = transactions.Select(x => x.DateOfTransaction.Year).Distinct().ToArray();
            for (int i = 0; i < years.Length; i++)
            {
                double quantityValue = transactions.Where(x => x.DateOfTransaction.Year == years[i]).Sum(x=>x.Quantity);
                double amountValue = transactions.Where(x => x.DateOfTransaction.Year == years[i]).Sum(x => x.TotalCost);

                quantityRecords.Add(new ChartsModel
                {
                    Year = years[i],
                    Value = quantityValue,                    
                });

                amountRecords.Add(new ChartsModel
                {
                    Year = years[i],
                    Value = amountValue,
                });

            }
            ViewBag.quantity = JsonConvert.SerializeObject(quantityRecords);
            ViewBag.amount = JsonConvert.SerializeObject(amountRecords);

            return PartialView();
        }


    }
}