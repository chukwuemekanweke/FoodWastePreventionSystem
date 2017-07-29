using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Areas.Customers.Models
{
    public class ViewProductVM
    {
        public OnAuctionVM Product { get; set; }
        public List<OnAuctionVM> ProductsFromStore { get; set; }
        public List<OnAuctionVM> ProductsWithSimilarExpiryDate { get; set; }


    }
}