using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels
{
    public class ProfitForProduct
    {
        public string ProductName { get; set; }
        public double ActualProfit { get; set; }
        public double ExpectedProfit { get; set; }
    }

    public class SaleForProduct
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
    }

    public class ProfitsForProductInYear
    {
        public int Year { get; set; }
        public Dictionary<Month, double> ProfitsPerMonth { get; set; }

        public ProfitsForProductInYear()
        {
            ProfitsPerMonth = new Dictionary<Month, double>();
        }

    }

    public class ProfitsForProduct
    {
        public Product Product { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<ProfitsForProductInYear> ProfitsForProductInYear { get; set; }
        public int YearSpan { get; set; }

        public ProfitsForProduct()
        {
            ProfitsForProductInYear = new List<ProfitsForProductInYear>();
        }
    }

    public class TransactionsForProductInYear
    {
        public int Year { get; set; }
        public Dictionary<Month, double> TransactionsPerMonth { get; set; }
    }

    public class TransactionsForProduct
    {
        public Product Product { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<TransactionsForProductInYear> TransactionsForProductInYear { get; set; }
        public int YearSpan { get; set; }

        public TransactionsForProduct()
        {
            TransactionsForProductInYear = new List<TransactionsForProductInYear>();
        }
    }

    public class TurnoverForProductInYear
    {
        public int Year { get; set; }
        public Dictionary<Month, double> TurnoverPerMonth { get; set; }
    }

    public class TurnoverForProduct
    {
        public Product Product { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<TurnoverForProductInYear> TurnoverForProductInYear { get; set; }
        public int YearSpan { get; set; }

        public TurnoverForProduct()
        {
            TurnoverForProductInYear = new List<TurnoverForProductInYear>();
        }
    }

    public class LossForProductInYear
    {
        public int Year { get; set; }
        public Dictionary<Month, double> LossPerMonth { get; set; }

        public LossForProductInYear() {
            LossPerMonth = new Dictionary<Month, double>();
        }


    }

    public class LossForProduct
    {
        public Product Product { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<LossForProductInYear> LossForProductInYear { get; set; }
        public int YearSpan { get; set; }

        public LossForProduct()
        {
            LossForProductInYear = new List<LossForProductInYear>();
        }
    }

    //public class ProfitLossInYear
    //{

    //}

    public class SalesAuctionViewModel
    {
        public int Year { get; set; }
        public List<SalesAuctionMonthlyViewModel> Record { get; set; }
    }

    public class SalesAuctionMonthlyViewModel
    {
        public Month Month { get; set; }
        public double SalesQuantity { get; set; }
        public double AuctionQuantity { get; set; }
    }

    public class ProfitLossVM
    {
        public int Year { get; set; }
        public List<ProfitLossMonthlyVM> Records { get; set; }

        public ProfitLossVM()
        {
            Records = new List<ProfitLossMonthlyVM>();
        }
    }

    public class ProfitLossMonthlyVM
    {
        public Month Month { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }
    }


    public class ProfitLossForBatch
    {
        public Batch Batch { get; set; }
        public List<Transaction> Transactions { get; set; }
        public double Value { get; set; }
        public BatchProfitLoss State { get; set; }
    }

    public class OnAuctionVM
    {
        public Product Product { get; set; }
        public Batch Batch { get; set; }
        public Auction Auction { get; set; }
    }

    public class StoreInformation
    {
        public Store Store { get; set; }
        public List<ApplicationUser> StoreManagers { get; set; }

        public StoreInformation()
        {
            StoreManagers = new List<ApplicationUser>();
        }
    }
}