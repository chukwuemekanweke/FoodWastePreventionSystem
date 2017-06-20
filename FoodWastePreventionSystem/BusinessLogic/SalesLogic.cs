using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;


namespace FoodWastePreventionSystem.BusinessLogic
{
    public class SalesLogic : Logic
    {

        public SalesLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {

        }

        public Transaction[] TransactionForProductByCriteria(Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {
            return TransactionRepo.GetAll().Where(TransactionPredicate).ToArray();
        }


        public Dictionary<TransactionType, int> GetSalesToAuctionInformationForProduct(Guid ProductId)
        {
            Product Product = ProductRepo.Get(x => x.Id == ProductId);
            Transaction[] SalesTransactions = TransactionForProductByCriteria(x => x.TransactionType == TransactionType.Sale);
            Transaction[] AuctionTransactions = TransactionForProductByCriteria(x => x.TransactionType == TransactionType.Auction);

            int SalesQuantity = SalesTransactions.Sum(x => x.Quantity);
            int AuctionQuantity = AuctionTransactions.Sum(x => x.Quantity);

            return new Dictionary<TransactionType, int>()
            {
                {TransactionType.Sale,SalesQuantity },
                {TransactionType.Auction,AuctionQuantity },
            };
        }


        public Dictionary<string, Dictionary<TransactionType, int>> GetSalesToAuctionInformationForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            Dictionary<string, Dictionary<TransactionType, int>> SalesToAuctionInfoForProducts = new Dictionary<string, Dictionary<TransactionType, int>>();
            Product[] Product = ProductPredicate == null ? ProductRepo.GetAll().ToArray() :
                                                           ProductRepo.GetAll().Where(ProductPredicate).ToArray();
            foreach (var item in Product)
            {
                SalesToAuctionInfoForProducts.Add(item.Name, GetSalesToAuctionInformationForProduct(item.Id));
            }
            return SalesToAuctionInfoForProducts;
        }


        public double GetSaleQuantityForProduct(Guid productId, int year = 0, Month month = Month.None)
        {
            SalesForProduct SalesRecords = GetSalesForProduct(productId);
            SalesForProductInYear SalesRecordForYear = SalesRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == year);

            return year == 0 ? SalesRecords.SalesForProductInYear.Sum(x => x.SalesPerMonth.Sum(y => y.Value)) : (month == Month.None ? SalesRecordForYear.SalesPerMonth.Sum(x => x.Value) : SalesRecordForYear.SalesPerMonth[month]);

        }


        public List<SalesForProduct> GetSalesForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            List<SalesForProduct> SaleRecordsForProducts = new List<SalesForProduct>();
            IEnumerable<Product> Products = ProductPredicate == null ? ProductRepo.GetAll() : ProductRepo.GetAll().Where(ProductPredicate);

            foreach (var item in Products)
            {
                SaleRecordsForProducts.Add(GetSalesForProduct(item.Id));
            }
            return SaleRecordsForProducts;

        }



        public SalesForProduct GetSalesForProduct(Guid productId, Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {
            Product Product = ProductRepo.Get(x => x.Id == productId);

            SalesForProduct SalesRecords = new SalesForProduct();

            Batch[] ProductInStoreRecords = BatchRepo.GetAll(x => x.ProductId == productId).ToArray();
            Guid[] ProductInStoreIds = ProductInStoreRecords.Select(x => x.Id).ToArray();

            Transaction[] TransactionsForProduct = TransactionPredicate == null ? TransactionRepo.GetAll(x => x.Batch.ProductId == productId).OrderBy(x => x.DateOfTransaction).ToArray() : TransactionRepo.GetAll(x => x.Batch.ProductId == productId).Where(TransactionPredicate).OrderBy(x => x.DateOfTransaction).ToArray();

            Dictionary<int, Dictionary<Month, double>> SalesPerYear = new Dictionary<int, Dictionary<Month, double>>();
            SalesPerYear = GroupSalesPerYear(TransactionsForProduct);

            if (SalesPerYear != null)
            {
                foreach (var item in SalesPerYear)
                {
                    int Year = item.Key;
                    Dictionary<Month, double> SalesPerMonth = item.Value;
                    SalesRecords.SalesForProductInYear.Add(new SalesForProductInYear() { Year = Year, SalesPerMonth = SalesPerMonth });
                }
                SalesRecords.Product = Product;

                return SalesRecords;
            }

            return null;
        }


        public Dictionary<int, Dictionary<Month, double>> GroupSalesPerYear(Transaction[] tansactions)
        {
            if (tansactions.Length > 0)
            {
                Dictionary<int, Dictionary<Month, double>> SalesPerYear = new Dictionary<int, Dictionary<Month, double>>();
                DateTime StartDate = tansactions.FirstOrDefault().DateOfTransaction;
                DateTime EndDate = tansactions.LastOrDefault().DateOfTransaction;
                int StartYear = StartDate.Year;
                int EndYear = EndDate.Year;
                int YearSpan = EndYear - StartYear;

                for (int i = StartYear; i <= EndYear; ++i)
                {
                    Transaction[] TransactionForYear = tansactions.Where(x => x.DateOfTransaction.Year == i).ToArray();
                    Dictionary<Month, double> SalesPerMonth = new Dictionary<Month, double>();
                    foreach (var item in TransactionForYear)
                    {

                        switch (item.DateOfTransaction.Month)
                        {
                            case 1:
                                if (SalesPerMonth.ContainsKey(Month.January))
                                {
                                    SalesPerMonth[Month.January] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.January, item.Quantity);
                                }
                                break;
                            case 2:
                                if (SalesPerMonth.ContainsKey(Month.February))
                                {
                                    SalesPerMonth[Month.February] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.February, item.Quantity);
                                }
                                break;
                            case 3:
                                if (SalesPerMonth.ContainsKey(Month.March))
                                {
                                    SalesPerMonth[Month.March] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.March, item.Quantity);
                                }
                                break;
                            case 4:
                                if (SalesPerMonth.ContainsKey(Month.April))
                                {
                                    SalesPerMonth[Month.April] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.April, item.Quantity);
                                }
                                break;
                            case 5:
                                if (SalesPerMonth.ContainsKey(Month.May))
                                {
                                    SalesPerMonth[Month.May] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.May, item.Quantity);
                                }
                                break;
                            case 6:
                                if (SalesPerMonth.ContainsKey(Month.June))
                                {
                                    SalesPerMonth[Month.June] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.June, item.Quantity);
                                }
                                break;
                            case 7:
                                if (SalesPerMonth.ContainsKey(Month.July))
                                {
                                    SalesPerMonth[Month.July] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.July, item.Quantity);
                                }
                                break;
                            case 8:
                                if (SalesPerMonth.ContainsKey(Month.August))
                                {
                                    SalesPerMonth[Month.August] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.August, item.Quantity);
                                }
                                break;
                            case 9:
                                if (SalesPerMonth.ContainsKey(Month.September))
                                {
                                    SalesPerMonth[Month.September] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.September, item.Quantity);
                                }
                                break;
                            case 10:
                                if (SalesPerMonth.ContainsKey(Month.October))
                                {
                                    SalesPerMonth[Month.October] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.October, item.Quantity);
                                }
                                break;
                            case 11:
                                if (SalesPerMonth.ContainsKey(Month.November))
                                {
                                    SalesPerMonth[Month.November] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.November, item.Quantity);
                                }
                                break;
                            case 12:
                                if (SalesPerMonth.ContainsKey(Month.December))
                                {
                                    SalesPerMonth[Month.December] += item.Quantity;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.December, item.Quantity);
                                }
                                break;
                        }
                    }

                    SalesPerYear.Add(i, SalesPerMonth);

                }



                return SalesPerYear;
            }

            return null;
        }


        public List<SalesAuctionViewModel> GetSalesToAuctionRatio(Guid id)
        {
            List<SalesAuctionViewModel> TotalRatioRecords = new List<SalesAuctionViewModel>();


            SalesForProduct SalesRecord = GetSalesForProduct(id, x => x.TransactionType == TransactionType.Sale);
            SalesForProduct AuctionRecords = GetSalesForProduct(id, x => x.TransactionType == TransactionType.Auction);

            if (SalesRecord != null && AuctionRecords != null)
            {
                int StartYear = SalesRecord.StartYear < AuctionRecords.StartYear ? SalesRecord.StartYear : AuctionRecords.StartYear;
                int EndYear = SalesRecord.EndYear > AuctionRecords.EndYear ? SalesRecord.EndYear : AuctionRecords.EndYear;

                for (int i = StartYear; i <= EndYear; i++)
                {
                    SalesAuctionViewModel YearlyRatioRecord = new SalesAuctionViewModel()
                    {
                        Year = i,
                    };

                    for (Month j = Month.January; j <= Month.December; ++j)
                    {

                        switch (j)
                        {
                            case Month.January:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.January);
                                break;

                            case Month.February:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.February);

                                break;

                            case Month.March:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.March);
                                break;

                            case Month.April:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.April);
                                break;

                            case Month.May:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.May);
                                break;

                            case Month.June:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.June);
                                break;

                            case Month.July:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.July);
                                break;

                            case Month.August:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.August);
                                break;

                            case Month.September:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.September);
                                break;

                            case Month.October:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.October);
                                break;

                            case Month.November:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.November);
                                break;

                            case Month.December:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.December);
                                break;


                        }
                    }
                    TotalRatioRecords.Add(YearlyRatioRecord);

                }
            }

            return TotalRatioRecords.Count > 0 ? TotalRatioRecords : null;
        }

        public void CompileMonthlySalesAuctionRatio(ref SalesAuctionViewModel monthlyRatioRecord, SalesForProduct salesRecords, SalesForProduct auctionRecords, Month month)
        {
            int Year = monthlyRatioRecord.Year;
            double SalesQuantity = salesRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == Year).SalesPerMonth[month];
            double AuctionQuantity = auctionRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == Year).SalesPerMonth[month];

            monthlyRatioRecord.Record.Add(new SalesAuctionMonthlyViewModel()
            {
                Month = Month.February,
                AuctionQuantity = AuctionQuantity,
                SalesQuantity = SalesQuantity,
            });
        }

        //MessedUp Code...Logic shouldnt have write actions to database
        public async Task<Transaction> AddSalesTransactionAsync(Transaction model, string agentId)
        {
            Transaction TransactionRecord = model;
            Batch Batch = BatchRepo.Get(c => c.Id == model.BatchId);
            Product Product = Batch.Product;
            if (TransactionRecord.BulkPurchase)
            {
                TransactionRecord.Quantity *= Product.QuantityPerCarton;
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice;
            }
            else
            {
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice * (Product.BulkPurchaseDiscountPercent / 100);
            }
            TransactionRecord.AgentId = agentId;
            TransactionRecord.TransactionType = TransactionType.Sale;
            Batch.QuantitySold += TransactionRecord.Quantity;

            await BatchRepo.UpdateAsync(Batch);
            return await TransactionRepo.AddAsync(TransactionRecord);

        }

    }
}