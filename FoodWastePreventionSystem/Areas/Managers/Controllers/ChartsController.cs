using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Managers.Controllers
{
    [Authorize]
    [RouteArea("Managers")]
    [RoutePrefix("Charts")]
    public class ChartsController : Controller
    {

        private IRepository<Product> ProductRepo;

        private SalesLogic SalesLogic;
        private ProfitLogic ProfitLogic;
        ChartLogic ChartLogic;


        public ChartsController(IRepository<Product> _ProductRepo, IRepository<FoodWastePreventionSystem.Models.Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo)
        {
            ProductRepo = _ProductRepo;

            SalesLogic = new SalesLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);
            ProfitLogic = new ProfitLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);
            ChartLogic = new ChartLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);

        }



        // GET: Managers/Charts
        //[HttpPost]
        //[Route("FetchTransactionDataForProduct")]
        //public JsonResult FetchTransactionDataForProduct(Guid id)
        //{

        //    return json(data.length > 0 ? data : null, jsonrequestbehavior.allowget);


        //    List<SalesAuctionYearlyChart> data = new List<SalesAuctionYearlyChart>() {
        //        new SalesAuctionYearlyChart() { year=2012, Auction=50, Sales=70},
        //        new SalesAuctionYearlyChart() { year=2013, Auction=90, Sales=7},
        //        new SalesAuctionYearlyChart() { year=2014, Auction=10, Sales=40},
        //        new SalesAuctionYearlyChart() { year=2015, Auction=20, Sales=60},
        //        new SalesAuctionYearlyChart() { year=2016, Auction=45, Sales=74},
        //        new SalesAuctionYearlyChart() { year=2017, Auction=10, Sales=23},
        //        new SalesAuctionYearlyChart() { year=2018, Auction=28, Sales=12},
        //        new SalesAuctionYearlyChart() { year=2019, Auction=40, Sales=65},

        //    };


        //    return Json(data.Count > 0 ? data : null, JsonRequestBehavior.AllowGet);




        //}

        #region Chart


        public ActionResult ProfitChart(ReqColumnChart chartData) =>  
            PartialView("Chart", ChartLogic.ProfitChart(chartData));
       

        public ActionResult LossChart(ReqColumnChart chartData) => 
            PartialView("Chart", ChartLogic.LossChart(chartData));
        

        public ActionResult SalesToAuctionChart(ReqColumnChart chartData)
        {
            Chart Chart = ChartLogic.SalesAuctionChart(chartData);
            Debug.WriteLine(Chart == null?"true":"false");
            return PartialView("Chart", Chart);
            //List<SalesAuctionYearlyChart> data = new List<SalesAuctionYearlyChart>() {
            //    new SalesAuctionYearlyChart() { year=2012, Auction=50, Sales=70},
            //    new SalesAuctionYearlyChart() { year=2013, Auction=90, Sales=7},
            //    new SalesAuctionYearlyChart() { year=2014, Auction=10, Sales=40},
            //    new SalesAuctionYearlyChart() { year=2015, Auction=20, Sales=60},
            //    new SalesAuctionYearlyChart() { year=2016, Auction=45, Sales=74},
            //    new SalesAuctionYearlyChart() { year=2017, Auction=10, Sales=23},
            //    new SalesAuctionYearlyChart() { year=2018, Auction=28, Sales=12},
            //    new SalesAuctionYearlyChart() { year=2019, Auction=40, Sales=65},

            //};


        }

        public ActionResult ProfitLossChart(ReqColumnChart chartData) => 
            PartialView("Chart", ChartLogic.SalesAuctionChart(chartData));       
           

        #endregion
    }
}