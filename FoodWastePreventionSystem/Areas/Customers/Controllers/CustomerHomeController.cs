using FoodWastePreventionSystem.Areas.Customers.Models;
using FoodWastePreventionSystem.BusinessLogic;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Customers.Controllers
{

    [Authorize]
    [RouteArea("Customers")]
    [RoutePrefix("CustomerHome")]
    public class CustomerHomeController : Controller
    {
        IRepository<Auction> AuctionRepo { get; set; }
        IRepository<Product> ProductRepo { get; set; }
        AuctionLogic AuctionLogic { get; set; }

        int PageSize = 6;

        public CustomerHomeController(IRepository<Auction> _Auction,IRepository<Product> _Product,
            AuctionLogic _AuctionL)
        {
            AuctionRepo = _Auction;
            AuctionLogic = _AuctionL;
            ProductRepo = _Product;
        }


        [Route("Index/{searchTerm}/{page:int}")]
        [Route("Index/{searchTerm}")]
        [Route("Index/{page:int}")]
        [Route("Index")]
        [Route("")]
        public ActionResult Index(string searchTerm, int? page)
        {
             List<OnAuctionVM> AuctionResult = new List<OnAuctionVM>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                AuctionResult = AuctionLogic.ViewProductsOnAuction();
            }
            else
            {
                ViewBag.SearchTerm = searchTerm;
                var Records = AuctionLogic.ViewProductsOnAuction();
                AuctionResult = Records.Where(x => x.Product.Name.Contains(searchTerm)).ToList();
            }
            int pageNumber = (page ?? 1);           
            return View(AuctionResult.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult ViewProduct(Guid id)
        {
            ViewProductVM Record = new ViewProductVM();
            Auction Auction =  AuctionRepo.Get(x => x.Id == id);
            var AvailabelAuctions = AuctionLogic.ViewProductsOnAuction();
              var OtherAuctionsByStore =   AvailabelAuctions
                                          .Where(x => x.Batch.Product.StoreId == Auction.Batch.Product.StoreId)
                                          .ToList();

            var AuctionsWithSimilarExpiryDate = AvailabelAuctions
                                                .Where(x => x.Batch.ExpiryDate == Auction.Batch.ExpiryDate)
                                                .ToList();

            OnAuctionVM ProductAuction = new OnAuctionVM()
            {
                Product=Auction.Batch.Product,
                Auction = Auction,
                Batch = Auction.Batch
            };

            Record.Product = ProductAuction;
            Record.ProductsFromStore = OtherAuctionsByStore;
            Record.ProductsWithSimilarExpiryDate = AuctionsWithSimilarExpiryDate;

            return View(Record);
        }
    }
}