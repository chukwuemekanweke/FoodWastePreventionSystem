using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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

        //Dictionary<string, double> TestRecord = new Dictionary<string, double>()
        //{
        //    ["2010"] = 200,
        //    ["2011"] = 114,
        //    ["2012"] = 250,
        //    ["2013"] = 250,
        //    ["2014"] = 250,
        //    ["2015"] = 150,
        //    ["2016"] = 350,
        //    ["2017"] = 550,
        //    ["2018"] = 050,
        //    ["2019"] = 210,
        //    ["2020"] = 255,
        //};

        //Dictionary<string, Dictionary<string, double>> TestRecord2 = new Dictionary<string, Dictionary<string, double>>()
        //{
        //    ["Omo"] = new Dictionary<string, double>()
        //    {
        //        ["2010"] = 20,
        //        ["2012"] = 20,
        //        ["2013"] = 50,
        //        ["2014"] = 20,
        //    },
        //    ["Indomie"] = new Dictionary<string, double>()
        //    {
        //        ["2010"] = 200,
        //        ["2011"] = 154,
        //        ["2012"] = 230,
        //        ["2013"] = 400,
        //        ["2014"] = 100,
        //    },
        //    ["Cowbell"] = new Dictionary<string, double>()
        //    {
        //        ["2010"] = 90,
        //        ["2011"] = 114,
        //        ["2012"] = 250,
        //        ["2013"] = 500,
        //        ["2014"] = 30,
        //    },
        //};

        //[Route("GetTestRecord/{id}/{operation}")]
        //public JsonResult GetTestRecord(string id, string operation)
        //{
        //    return Json(TestRecord, JsonRequestBehavior.AllowGet);
        //}


        //[Route("GetTestRecord2/{operation}/{category}")]
        //[Route("GetTestRecord2/{operation}")]
        //public JsonResult GetTestRecord2(string operation, string category="")
        //{
        //    return Json(TestRecord2, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Index1()
        {
            return View();
        }






        private IRepository<Product> ProductRepo;

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


        public ChartsController(IRepository<Product> _ProductRepo,  SalesLogic _SalesL, ProfitLogic _ProfitL, ChartLogic _ChartL)
        {
            ProductRepo = _ProductRepo;
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








        //[HttpGet]
        //[Route("FetchYearSpanForSpecifiedProduct/{productId}/dataType")]
        //public JsonResult FetchYearSpanForSpecifiedProduct(string productId, string dataType)
        //{
        //    Guid Id = Guid.Parse(productId);
        //    List<string> Years = new List<string>();
        //    Dictionary<string, double> Records = new Dictionary<string, double>();
        //    switch (dataType)
        //    {
        //        case "profit":
        //            Records = ChartLogic.ProfitForProduct(Id);
        //            break;
        //        case "sales":
        //            Records = ChartLogic.SalesForProduct(Id);
        //            break;
        //        case "auction":
        //            Records = ChartLogic.AuctionsForProduct(Id);
        //            break;
        //    }

        //    foreach (var record in Records)
        //    {
        //        if (!Years.Contains(record.Key))
        //        {
        //            Years.Add(record.Key);
        //        }
        //    }

        //    return Json(Years, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult FetchYearSpanForProducts(string dataType, string category = "")
        //{
        //    List<string> Years = new List<string>();
        //    Dictionary<string, Dictionary<string, double>> Records = new Dictionary<string, Dictionary<string, double>>();
        //    switch (dataType)
        //    {
        //        case "profit":
        //            Records = string.IsNullOrWhiteSpace(category) ? ChartLogic.ProfitForProducts() : ChartLogic.ProfitForProducts(x => x.Category == category);

        //            break;
        //        case "sales":
        //            Records = string.IsNullOrWhiteSpace(category) ? ChartLogic.SalesForProducts() : ChartLogic.SalesForProducts(x => x.Category == category);
        //            break;
        //        case "auction":
        //            Records = string.IsNullOrWhiteSpace(category) ? ChartLogic.AuctionsForProducts() : ChartLogic.AuctionsForProducts(x => x.Category == category);

        //            break;
        //    }

        //    foreach (var record in Records)
        //    {
        //        string[] years = record.Value.Keys.Distinct().ToArray();
        //        foreach (var item in years)
        //        {
        //            if (!years.Contains(item))
        //            {
        //                Years.Add(item);
        //            }
        //        }

        //    }

        //    return Json(Years, JsonRequestBehavior.AllowGet);
        //}




    }
}