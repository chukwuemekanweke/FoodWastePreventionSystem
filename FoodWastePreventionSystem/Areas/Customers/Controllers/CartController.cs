using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FoodWastePreventionSystem.Areas.Customers.Controllers
{

    [Authorize(Roles = "customer")]
    [RouteArea("Customers")]
    [RoutePrefix("Cart")]
    public class CartController : Controller
    {
       private  IRepository<Auction> AuctionRepo { get; set; }
        private IRepository<Cart> CartRepo { get; set; }
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
                return  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
               
            }
        }



        public CartController(IRepository<Auction> _Auction, IRepository<Cart> _Cart, IRepository<Transaction> _TransactionRepo)
        {
            AuctionRepo = _Auction;
            CartRepo = _Cart;
            TransactionRepo = _TransactionRepo;
        }


        public ActionResult Index()
        {
            return View();
        }

        // GET: Customers/Cart
        public ActionResult MyCart()=>            
             View(CartRepo.GetAll(x => x.CustomerId == LoggedInUser.Id.ToString()).ToList());
        
        [HttpGet]
        public ActionResult AddToCart(Guid id, int quantity)
        {
            int Quantity = quantity;       
            Auction Auction = AuctionRepo.Get(x => x.Id == id);
            Batch Batch = Auction.Batch;
            int AvailableUnits = Batch.QuantityPurchased - (Batch.QuantityAuctioned + Batch.QuantityDisposedOf + Batch.QuantitySold);
            if (Quantity < AvailableUnits)
            {
                Cart CurrentCart = CartRepo.Get(x => x.Auction.BatchId == Auction.BatchId && x.CustomerId == LoggedInUser.Id);
                if (CurrentCart != null)
                {
                    CurrentCart.Quantity += Quantity;
                    CartRepo.Update(CurrentCart);
                }
                else
                {
                    CartRepo.Add(new Cart()
                    {
                        AuctionId = Auction.Id,
                        Quantity = Quantity,
                        CustomerId = LoggedInUser.Id
                    });
                }
            }
            else
            {
                return RedirectToAction("StockUnavailable");
            }

            return RedirectToAction("MyCart");
        }

        [HttpPost]
        public ActionResult AddToCart(FormCollection collection)
        {
            int Quantity =int.Parse(collection["quantity"]);
            Guid id = Guid.Parse(collection["id"]);
            Auction Auction = AuctionRepo.Get(x => x.Id == id);
            Batch Batch = Auction.Batch;
            int AvailableUnits = Batch.QuantityPurchased - (Batch.QuantityAuctioned + Batch.QuantityDisposedOf + Batch.QuantitySold);
            if (Quantity < AvailableUnits)
            {
                Cart CurrentCart = CartRepo.Get(x => x.Auction.BatchId == Auction.BatchId && x.CustomerId == LoggedInUser.Id);
                if (CurrentCart != null)
                {
                    CurrentCart.Quantity += Quantity;
                    CartRepo.Update(CurrentCart);
                }
                else
                {
                    CartRepo.Add(new Cart()
                    {
                        AuctionId = Auction.Id,
                        Quantity = Quantity,
                        CustomerId = LoggedInUser.Id
                    });
                }
            }
            else
            {
                return RedirectToAction("StockUnavailable");
            }

            return RedirectToAction("MyCart");
        }

        [AllowAnonymous]
        public ActionResult CartUpdates()=>        
            PartialView(CartRepo.GetAll(x => x.CustomerId == LoggedInUser.Id.ToString()).ToList());

        
        [HttpPost]
        public async Task<ActionResult> Checkout(FormCollection collection)
        {
            var email = LoggedInUser.Email;
            var total = int.Parse(collection["total"]) * 100;
            var Payment = new Payment("sk_test_5f0518d7b4135b7b70bd07f384b331286f06d843");

            var Transaction = await Payment.InitializeTransaction(total, "emekanweke604@gmail.com");
            if (Transaction.status)
            {
                return RedirectPermanent(Transaction.data.authorization_url);
                //return Json(Transaction, JsonRequestBehavior.AllowGet);
            }
            return Content("An Error Occured");
        }

        public async Task<ActionResult> VerifyTransaction(string id)
        {
            string reference = id;
            string response = await new Payment("sk_test_5f0518d7b4135b7b70bd07f384b331286f06d843").VerifyTransaction(reference);
            return Content(response);
        }

        [HttpGet]
        [Route("TransactionSuccessful")]
        public ActionResult TransactionSuccessful(string trxref,string reference)
        {
            Transaction Transaction = new Transaction();
            string UserId = LoggedInUser.Id.ToString();
            List<Cart> CartTransactions = CartRepo.GetAll(x => x.CustomerId == UserId).ToList();
            foreach (var item in CartTransactions)
            {
                TransactionRepo.Add(new Transaction()
                {
                    AuctioneeId = UserId,
                    BatchId = item.Auction.BatchId,
                    BulkPurchase = false,
                    DateOfTransaction = DateTime.Now,
                    Quantity = item.Quantity,
                    TransactionType = TransactionType.Auction,
                    TotalCost = item.Auction.AuctionPrice * item.Quantity,
                });
            }

            Guid[] ids = CartTransactions.Select(x => x.Id).ToArray();

            foreach (var item in ids)
            {
                CartRepo.Delete(item);
            }

            return View();
        }

        public ActionResult RemoveProductFromCart(Guid id)
        {
            CartRepo.Delete(id);
            return RedirectToAction("MyCart");
        }
    }
}