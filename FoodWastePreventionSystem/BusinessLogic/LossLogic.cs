using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class LossLogic : Logic
    {
        public LossLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {

        }

        public double GetLossAmountForProduct(Guid productId, int year=0, Month month=Month.None)
        {
            LossForProduct LossRecords = GetLossForProduct(productId);
            LossForProductInYear LossRecordForYear = LossRecords.LossForProductInYear.FirstOrDefault(x => x.Year == year);

           return  year == 0 ? LossRecords.LossForProductInYear.Sum(x => x.LossPerMonth.Sum(y => y.Value)): (month==Month.None?LossRecordForYear.LossPerMonth.Sum(x => x.Value):LossRecordForYear.LossPerMonth[month]);

        }


        public LossForProduct GetLossForProduct(Guid productId)
        {
            Product Product = ProductRepo.Get(x => x.Id == productId);
            LossForProduct LossRecords = new LossForProduct();


            Batch[] ProductInStoreRecords = BatchRepo.GetAll(x => x.ProductId == productId).ToArray();
            Guid[] ProductInStoreIds = ProductInStoreRecords.Select(x => x.Id).ToArray();
            Loss[] LossesForProduct = LossRepo.GetAll(x => x.Batch.ProductId == productId).OrderBy(x => x.DateOfLoss).ToArray();

            Dictionary<int, Dictionary<Month, double>> LossPerYear = new Dictionary<int, Dictionary<Month, double>>();
            LossPerYear = GroupLossPerYear(LossesForProduct);

            foreach (var item in LossPerYear)
            {
                int Year = item.Key;
                Dictionary<Month, double> LossPerMonth = item.Value;
                LossRecords.LossForProductInYear.Add(new LossForProductInYear() { Year = Year, LossPerMonth = LossPerMonth, });

            }
            LossRecords.Product = Product;
            return LossRecords;
        }

        private Dictionary<int, Dictionary<Month, double>> GroupLossPerYear(Loss[] lossesForProduct)
        {
            Dictionary<int, Dictionary<Month, double>> LossPerYear = new Dictionary<int, Dictionary<Month, double>>();
            DateTime StartDate = lossesForProduct.First().DateOfLoss;
            DateTime EndDate = lossesForProduct.Last().DateOfLoss;
            int StartYear = StartDate.Year;
            int EndYear = EndDate.Year;
            int YearSpan = EndYear - StartYear;

            for (int i = StartYear; i <= EndYear; ++i)
            {
                Loss[] TransactionForYear = lossesForProduct.Where(x => x.DateOfLoss.Year == i).ToArray();
                Dictionary<Month, double> LossPerMonth = new Dictionary<Month, double>();
                foreach (var item in TransactionForYear)
                {

                    switch (item.DateOfLoss.Month)
                    {
                        case 1:
                            if (LossPerMonth.ContainsKey(Month.January))
                            {
                                LossPerMonth[Month.January] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.January, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 2:
                            if (LossPerMonth.ContainsKey(Month.February))
                            {
                                LossPerMonth[Month.February] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.February, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 3:
                            if (LossPerMonth.ContainsKey(Month.March))
                            {
                                LossPerMonth[Month.March] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.March, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 4:
                            if (LossPerMonth.ContainsKey(Month.April))
                            {
                                LossPerMonth[Month.April] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.April, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 5:
                            if (LossPerMonth.ContainsKey(Month.May))
                            {
                                LossPerMonth[Month.May] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.May, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 6:
                            if (LossPerMonth.ContainsKey(Month.June))
                            {
                                LossPerMonth[Month.June] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.June, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 7:
                            if (LossPerMonth.ContainsKey(Month.July))
                            {
                                LossPerMonth[Month.July] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.July, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 8:
                            if (LossPerMonth.ContainsKey(Month.August))
                            {
                                LossPerMonth[Month.August] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.August, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 9:
                            if (LossPerMonth.ContainsKey(Month.September))
                            {
                                LossPerMonth[Month.September] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.September, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 10:
                            if (LossPerMonth.ContainsKey(Month.October))
                            {
                                LossPerMonth[Month.October] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.October, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 11:
                            if (LossPerMonth.ContainsKey(Month.November))
                            {
                                LossPerMonth[Month.November] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.November, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                        case 12:
                            if (LossPerMonth.ContainsKey(Month.December))
                            {
                                LossPerMonth[Month.December] += item.Quantity * item.Batch.PurchasePrice;
                            }
                            else
                            {
                                LossPerMonth.Add(Month.December, item.Quantity * item.Batch.PurchasePrice);
                            }
                            break;
                    }
                }
                LossPerYear.Add(i, LossPerMonth);
            }
            return LossPerYear;
        }
    }
}