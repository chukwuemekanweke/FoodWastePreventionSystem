using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class ChartLogic : Logic
    {
        private ProfitLogic ProfitLogic;
        private SalesLogic SalesLogic;
        private LossLogic LossLogic;
        private AuctionLogic AuctionLogic;
        ChartsRenderer ChartsRenderer;


        public ChartLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, 
            IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo,
            IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo,
            ProfitLogic _ProfitL, SalesLogic _SalesL, LossLogic _LossL, AuctionLogic _AuctionL) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {
            ProfitLogic = _ProfitL;
            SalesLogic = _SalesL;
            LossLogic = _LossL;
            AuctionLogic = _AuctionL;
            ChartsRenderer = new ChartsRenderer();
        }

        public Dictionary<string, double> ProfitForProduct(Guid productId)
        {
            Dictionary<string, double> Report = new Dictionary<string, double>();
            ProfitsForProduct ProfitRecord = ProfitLogic.GetProfitForProduct(productId);
            ProfitRecord.ProfitsForProductInYear.ForEach(x =>
            {
                Report.Add(x.Year.ToString(), x.ProfitsPerMonth.Sum(y => y.Value));
            });
            return Report;
        }

        public Dictionary<string,double> AnnualProfitForProduct(Guid productId, int year)
        {
            Dictionary<string, double> Report = new Dictionary<string, double>();
            ProfitsForProductInYear ProfitRecord = ProfitLogic.GetProfitForProduct(productId).ProfitsForProductInYear
                                                    .FirstOrDefault(x=>x.Year==year);
            if (ProfitRecord != null)
            {
                ProfitRecord.ProfitsPerMonth.Keys.ToList().ForEach(x=>{
                    Report.Add(x.ToString(), ProfitRecord.ProfitsPerMonth[x]);
                });
            }
            return Report;
        }

        public Dictionary<string, Dictionary<string, double>> ProfitForProducts(Expression<Func<Product, bool>> productPredicate = null)
        {
            Dictionary<string, Dictionary<string, double>> Report = new Dictionary<string, Dictionary<string, double>>();
            List<Product> Product = ProductRepo.GetAll(productPredicate).ToList();
            Product.ForEach(x => {
                Report.Add(x.Name, ProfitForProduct(x.Id));
            });
            return Report;
        }

        public Dictionary<string,Dictionary<string,double>> AnnualProfitForProducts(int year,Expression<Func<Product, bool>> productPredicate = null)
        {
            Dictionary<string, Dictionary<string, double>> Report = new Dictionary<string, Dictionary<string, double>>();
            List<Product> Product = ProductRepo.GetAll(productPredicate).ToList();
            Product.ForEach(x => {
                Report.Add(x.Name, AnnualProfitForProduct(x.Id,year));
            });
            return Report;
        }

        public Dictionary<string, double> SalesForProduct(Guid productId)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverRecords;
            Dictionary<string, double> Report = new Dictionary<string, double>();
            TransactionsForProduct SalesRecords = SalesLogic.GetSalesForProduct(productId, out TurnoverRecords);

            if (SalesRecords != null)
            {
                SalesRecords.TransactionsForProductInYear.ForEach(x =>
                {
                    Report.Add(x.Year.ToString(), x.TransactionsPerMonth.Sum(y => y.Value));
                });
            }
            return Report;
        }

        public Dictionary<string,double> AnnualSalesForProduct(Guid productId, int year)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverRecords;
            Dictionary<string, double> Report = new Dictionary<string, double>();
            TransactionsForProductInYear SalesRecord = SalesLogic.GetSalesForProduct(productId, out TurnoverRecords).TransactionsForProductInYear
                                                    .FirstOrDefault(x => x.Year == year);
            if (SalesRecord != null)
            {
                SalesRecord.TransactionsPerMonth.Keys.ToList().ForEach(x => {
                    Report.Add(x.ToString(), SalesRecord.TransactionsPerMonth[x]);
                });
            }
            return Report;
        }

        public Dictionary<string, Dictionary<string, double>> SalesForProducts(Expression<Func<Product, bool>> productPredicate = null)
        {
            Dictionary<string, Dictionary<string, double>> Report = new Dictionary<string, Dictionary<string, double>>();
            List<Product> Product = new List<Product>();
               Product =  ProductRepo.GetAll(productPredicate).ToList();
            Product.ForEach(x => {
                Report.Add(x.Name, SalesForProduct(x.Id));
            });
            return Report;
        }

        public Dictionary<string,Dictionary<string,double>> AnnualSalesForProducts(int year,Expression<Func<Product, bool>> productPredicate = null)
        {
            Dictionary<string, Dictionary<string, double>> Report = new Dictionary<string, Dictionary<string, double>>();
            List<Product> Product = ProductRepo.GetAll(productPredicate).ToList();
            Product.ForEach(x => {
                Report.Add(x.Name, AnnualProfitForProduct(x.Id,year));
            });
            return Report;
        }

         public Dictionary<string, double> AuctionsForProduct(Guid productId)
        {
            Dictionary<string, double> Report = new Dictionary<string, double>();
            TransactionsForProduct Auctionrecords = AuctionLogic.GetAuctionsForProduct(productId);
            Auctionrecords.TransactionsForProductInYear.ForEach(x => {
                Report.Add(x.Year.ToString(), x.TransactionsPerMonth.Sum(y => y.Value));
            });
            return Report;
        }

        public Dictionary<string, double> AnnualAuctionForProduct(Guid productId, int year)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverRecords;
            Dictionary<string, double> Report = new Dictionary<string, double>();
            TransactionsForProductInYear AuctionRecord = AuctionLogic.GetAuctionsForProduct(productId).TransactionsForProductInYear
                                                    .FirstOrDefault(x => x.Year == year);
            if (AuctionRecord != null)
            {
                AuctionRecord.TransactionsPerMonth.Keys.ToList().ForEach(x => {
                    Report.Add(x.ToString(), AuctionRecord.TransactionsPerMonth[x]);
                });
            }
            return Report;
        }

        public Dictionary<string, Dictionary<string, double>> AuctionsForProducts(Expression<Func<Product, bool>> productPredicate = null)
        {
            Dictionary<string, Dictionary<string, double>> Report = new Dictionary<string, Dictionary<string, double>>();
            List<Product> Product = ProductRepo.GetAll(productPredicate).ToList();
            Product.ForEach(x => {
                Report.Add(x.Name, AuctionsForProduct(x.Id));
            });
            return Report;
        }





        


    }
}