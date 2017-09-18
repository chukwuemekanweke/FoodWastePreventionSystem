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
    [AllowAnonymous]
    [RouteArea("Customers")]
    [RoutePrefix("CustomerHome")]
    public class CustomerHomeController : Controller
    {
        IRepository<Auction> AuctionRepo { get; set; }
        IRepository<Product> ProductRepo { get; set; }
        IRepository<Store> StoreRepo { get; set; }
        AuctionLogic AuctionLogic { get; set; }

        int PageSize = 6;

        public CustomerHomeController(IRepository<Auction> _Auction,IRepository<Product> _Product, IRepository<Store> _Store,
            AuctionLogic _AuctionL)
        {
            AuctionRepo = _Auction;
            AuctionLogic = _AuctionL;
            ProductRepo = _Product;
            StoreRepo = _Store;
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
                AuctionResult = Records.Where(x => x.Product.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
                if (AuctionResult.Count == 0)
                {
                    AuctionResult = Records.Where(x => x.Product.Category.ToLower().Contains(searchTerm.ToLower())).ToList();
                }
            }
            int pageNumber = (page ?? 1);           
            return View(AuctionResult.OrderByDescending(x=>x.Batch.ExpiryDate).ToPagedList(pageNumber, PageSize));
        }

        public ActionResult ViewProduct(Guid id)
        {
            ViewProductVM Record = new ViewProductVM();
            Auction Auction =  AuctionRepo.Get(x => x.Id == id);
            var AvailabelAuctions = AuctionLogic.ViewProductsOnAuction();
            var OtherAuctionsByStore =   AvailabelAuctions
                                          .Where(x => x.Batch.Product.StoreId == Auction.Batch.Product.StoreId && x.Batch.Id != Auction.BatchId)
                                          .ToList();

            var AuctionsWithSimilarExpiryDate = AvailabelAuctions
                                                .Where(x => x.Batch.ExpiryDate == Auction.Batch.ExpiryDate && x.Batch.Id!=Auction.BatchId)
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

        public ActionResult SortByStores()
        {
            ViewBag.stores = StoreRepo.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToList();

            return View();

        }

        
        public PartialViewResult ProductsByStore(Guid id)
        {
            List<OnAuctionVM> AuctionResult = new List<OnAuctionVM>();

            Store Store = StoreRepo.Get(x => x.Id == id);
            List<Auction> Auctions = new List<Auction>();
            Auctions = AuctionRepo.GetAll(x => x.Batch.Product.StoreId == id).ToList();

            foreach (var item in Auctions)
            {
                AuctionResult.Add(new OnAuctionVM() {
                    Auction= item,
                    Batch = item.Batch,
                    Product = item.Batch.Product
                });
            }

            ViewBag.storeName = Store.Name;

            ViewBag.categories = AuctionResult.Select(x => x.Product.Category).Distinct().ToList();
            return PartialView(AuctionResult);

        }
    }
}