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
    public class ProfitLogic : Logic
    {
        LossLogic LossLogic;

        public ProfitLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo,IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {
            LossLogic = new LossLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);
        }

        public double GetProfitAmountForProduct(Guid productId, int year = 0, Month month = Month.None)
        {
            ProfitsForProduct ProfitRecords = GetProfitForProduct(productId);
            ProfitsForProductInYear ProfitRecordForYear = ProfitRecords.ProfitsForProductInYear.FirstOrDefault(x => x.Year == year);

            return year == 0 ? ProfitRecords.ProfitsForProductInYear.Sum(x => x.ProfitsPerMonth.Sum(y => y.Value)) : (month == Month.None ? ProfitRecordForYear.ProfitsPerMonth.Sum(x => x.Value) : ProfitRecordForYear.ProfitsPerMonth[month]);

        }

        public List<ProfitsForProduct> GetProfitForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            List<ProfitsForProduct> ProfitRecordsForProducts = new List<ProfitsForProduct>();
            IEnumerable<Product> Products = ProductPredicate == null ? ProductRepo.GetAll() : ProductRepo.GetAll().Where(ProductPredicate);

            foreach (var item in Products)
            {
                ProfitRecordsForProducts.Add(GetProfitForProduct(item.Id));
            }
            return ProfitRecordsForProducts;

        }

        public ProfitsForProduct GetProfitForProduct(Guid productId, Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {
            Product Product = ProductRepo.Get(x => x.Id == productId);
            ProfitsForProduct ProfitRecords = new ProfitsForProduct();


            Batch[] ProductInStoreRecords = BatchRepo.GetAll(x => x.ProductId == productId).ToArray();
            Guid[] ProductInStoreIds = ProductInStoreRecords.Select(x => x.Id).ToArray();
            Transaction[] TransactionsForProduct = TransactionPredicate == null ? TransactionRepo.GetAll(x => x.Batch.ProductId == productId).OrderBy(x => x.DateOfTransaction).ToArray() : TransactionRepo.GetAll(x => x.Batch.ProductId == productId).Where(TransactionPredicate).OrderBy(x => x.DateOfTransaction).ToArray();

            Dictionary<int, Dictionary<Month, double>> ProfitsPerYear = new Dictionary<int, Dictionary<Month, double>>();
            ProfitsPerYear = GroupProfitsPerYear(TransactionsForProduct);

            foreach (var item in ProfitsPerYear)
            {
                int Year = item.Key;
                Dictionary<Month, double> ProfitsPerMonth = item.Value;
                ProfitRecords.ProfitsForProductInYear.Add(new ProfitsForProductInYear() { Year = Year, ProfitsPerMonth = ProfitsPerMonth, });

            }

            ProfitRecords.Product = Product;

            return ProfitRecords;

        }




        public Dictionary<int, Dictionary<Month, double>> GroupProfitsPerYear(Transaction[] tansactions)
        {
            Dictionary<int, Dictionary<Month, double>> TransactionsPerYear = new Dictionary<int, Dictionary<Month, double>>();
            DateTime StartDate = tansactions.First().DateOfTransaction;
            DateTime EndDate = tansactions.Last().DateOfTransaction;
            int StartYear = StartDate.Year;
            int EndYear = EndDate.Year;
            int YearSpan = EndYear - StartYear;

            for (int i = StartYear; i <= EndYear; ++i)
            {
                Transaction[] TransactionForYear = tansactions.Where(x => x.DateOfTransaction.Year == i).ToArray();
                Dictionary<Month, double> TransactionsPerMonth = new Dictionary<Month, double>();
                foreach (var item in TransactionForYear)
                {

                    switch (item.DateOfTransaction.Month)
                    {
                        case 1:
                            if (TransactionsPerMonth.ContainsKey(Month.January))
                            {
                                TransactionsPerMonth[Month.January] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.January, item.ProfitMade);
                            }
                            break;
                        case 2:
                            if (TransactionsPerMonth.ContainsKey(Month.February))
                            {
                                TransactionsPerMonth[Month.February] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.February, item.ProfitMade);
                            }
                            break;
                        case 3:
                            if (TransactionsPerMonth.ContainsKey(Month.March))
                            {
                                TransactionsPerMonth[Month.March] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.March, item.ProfitMade);
                            }
                            break;
                        case 4:
                            if (TransactionsPerMonth.ContainsKey(Month.April))
                            {
                                TransactionsPerMonth[Month.April] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.April, item.ProfitMade);
                            }
                            break;
                        case 5:
                            if (TransactionsPerMonth.ContainsKey(Month.May))
                            {
                                TransactionsPerMonth[Month.May] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.May, item.ProfitMade);
                            }
                            break;
                        case 6:
                            if (TransactionsPerMonth.ContainsKey(Month.June))
                            {
                                TransactionsPerMonth[Month.June] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.June, item.ProfitMade);
                            }
                            break;
                        case 7:
                            if (TransactionsPerMonth.ContainsKey(Month.July))
                            {
                                TransactionsPerMonth[Month.July] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.July, item.ProfitMade);
                            }
                            break;
                        case 8:
                            if (TransactionsPerMonth.ContainsKey(Month.August))
                            {
                                TransactionsPerMonth[Month.August] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.August, item.ProfitMade);
                            }
                            break;
                        case 9:
                            if (TransactionsPerMonth.ContainsKey(Month.September))
                            {
                                TransactionsPerMonth[Month.September] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.September, item.ProfitMade);
                            }
                            break;
                        case 10:
                            if (TransactionsPerMonth.ContainsKey(Month.October))
                            {
                                TransactionsPerMonth[Month.October] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.October, item.ProfitMade);
                            }
                            break;
                        case 11:
                            if (TransactionsPerMonth.ContainsKey(Month.November))
                            {
                                TransactionsPerMonth[Month.November] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.November, item.ProfitMade);
                            }
                            break;
                        case 12:
                            if (TransactionsPerMonth.ContainsKey(Month.December))
                            {
                                TransactionsPerMonth[Month.December] += item.ProfitMade;
                            }
                            else
                            {
                                TransactionsPerMonth.Add(Month.December, item.ProfitMade);
                            }
                            break;
                    }
                }

                TransactionsPerYear.Add(i, TransactionsPerMonth);

            }



            return TransactionsPerYear;
        }


        public List<ProfitLossVM> GetProfitLossRatio(Guid id)
        {
            List<ProfitLossVM> TotalRatioRecords = new List<ProfitLossVM>();
            ProfitsForProduct ProfitRecords = GetProfitForProduct(id);
            LossForProduct LossRecords = LossLogic.GetLossForProduct(id);

            int ProfitStartYear = ProfitRecords.StartYear;
            int ProfitsEndYear = ProfitRecords.EndYear;
            int LossStartYear = LossRecords.StartYear;
            int LossEndYear = LossRecords.EndYear;

            int StartYear = ProfitStartYear < LossStartYear ? ProfitStartYear : LossStartYear;
            int EndYear = ProfitsEndYear < LossEndYear ? LossEndYear : ProfitsEndYear;

            for (int i = StartYear; i <= EndYear; ++i)
            {
                ProfitLossVM YearlyRatioRecord = new ProfitLossVM { Year = i };

                for (Month j = Month.January; j <= Month.December; ++j)
                {
                    switch (j)
                    {
                        case Month.January:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.January);
                            break;

                        case Month.February:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.February);

                            break;

                        case Month.March:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.March);
                            break;

                        case Month.April:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.April);
                            break;

                        case Month.May:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.May);
                            break;

                        case Month.June:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.June);
                            break;

                        case Month.July:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.July);
                            break;

                        case Month.August:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.August);
                            break;

                        case Month.September:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.September);
                            break;

                        case Month.October:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.October);
                            break;

                        case Month.November:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.November);
                            break;

                        case Month.December:
                            CompileMonthlyProfitLossRatio(ref YearlyRatioRecord, ProfitRecords, LossRecords, Month.December);
                            break;


                    }
                }
                TotalRatioRecords.Add(YearlyRatioRecord);
            }

            return TotalRatioRecords;
        }

        private void CompileMonthlyProfitLossRatio(ref ProfitLossVM yearlyRatioRecord, ProfitsForProduct profitRecords, LossForProduct lossRecords, Month month)
        {
            int Year = yearlyRatioRecord.Year;
            double ProfitQuantity = profitRecords.ProfitsForProductInYear.FirstOrDefault(x => x.Year == Year).ProfitsPerMonth[month];
            double LossQuantity = lossRecords.LossForProductInYear.FirstOrDefault(x => x.Year == Year).LossPerMonth[month];

            yearlyRatioRecord.Records.Add(new ProfitLossMonthlyVM()
            {
                Month = Month.February,
                Profit = ProfitQuantity,
                Loss = LossQuantity,
            });
        }
    }
}