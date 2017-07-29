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

    [Authorize]
    [RouteArea("Customers")]
    [RoutePrefix("Cart")]
    public class CartController : Controller
    {
       private  IRepository<Auction> AuctionRepo { get; set; }
        private IRepository<Cart> CartRepo { get; set; }




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



        public CartController(IRepository<Auction> _Auction, IRepository<Cart> _Cart)
        {
            AuctionRepo = _Auction;
            CartRepo = _Cart;
        }

        // GET: Customers/Cart
        public ActionResult MyCart()=>            
             View(CartRepo.GetAll(x => x.CustomerId == LoggedInUser.Id.ToString()).ToList());
        

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
                Cart CurrentCart = CartRepo.Get(x => x.BatchId == Auction.BatchId && x.CustomerId == LoggedInUser.Id);
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
                        BatchId = Batch.Id,
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

        public ActionResult CartUpdates()=>        
            PartialView(CartRepo.GetAll(x => x.CustomerId == LoggedInUser.Id.ToString()).ToList());

        

        public async Task<ActionResult> Checkout()
        {
            var Payment = new Payment("");
            var Transaction = await Payment.InitializeTransaction(20, "emekanweke604@gmail.com");
            if (Transaction.status)
            {
                return RedirectPermanent(Transaction.data.authorization_url);
                //return Json(Transaction, JsonRequestBehavior.AllowGet);
            }
            return Content("An Error Occured");
        }
    }
}